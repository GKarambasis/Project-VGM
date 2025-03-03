using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Unity.VisualScripting;

public class VGMInputController : MonoBehaviour
{
    [Header("Mouse Indicator")]
    [SerializeField]
    GameObject mouseIndicator;
    [SerializeField]
    GameObject selectionPointer;


    [Header("RayCast Settings")]
    [SerializeField]
    private LayerMask placementLayermask; //
    [SerializeField] 
    private LayerMask selectedLayermask;
    [SerializeField]
    private float raycastDistance;


    [Header("Object Selection")]
    //button clicked in Creator Window that contains all the Object Information
    GameObject selectedObject;
    //reference to the Prefab contained within selectedObject
    GameObject selectedPrefab;

    public GameObject selectedPlacedObject;

    [Header("Other Script References")]
    [SerializeField] PanelHierarchy panelHierarchy;
    NPCEditor npcEditor;

    [Header("Private Variables")]
    private Camera sceneCamera;
    private Vector3 lastPosition; //Last Cursor Position


    private void Start()
    {
        npcEditor = FindObjectOfType<NPCEditor>(true);
        panelHierarchy = FindObjectOfType<PanelHierarchy>(true);
        sceneCamera = gameObject.GetComponent<Camera>();
    }
    private void Update()
    {
        //Place Object
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            OnClick_PlaceObject();
        }
        //Select an already Placed Object
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject() && selectedObject == null)
        {
            OnClick_SelectPlacedObject();
        }
        //Open the editor of an object
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClick_OpenNPCEditor();
        }
        
        SelectedPlacedObject();
    
        UpdateMouseIndicator();
    }

    //Updates the Position of the Mouse on the Terrain
    private void UpdateMouseIndicator()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            mouseIndicator.SetActive(false);
            return; // If over a UI element, do not perform the raycast
        }
        mouseIndicator.SetActive(true);
        Vector3 mousePosition = GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;
    }

    //Ray Cast that transforms mouse position to in-world position
    private Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition; //get the position of the mouse on the screen
        mousePos.z = sceneCamera.nearClipPlane; //Prevent Objects to be selected if they are not rendered by the camera
       
        //Raycast from mouse to game world fildered by placementMask
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, raycastDistance, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }


    
    //Method for placing down a selected Object from the Creator Menu
    private void OnClick_PlaceObject()
    {
        if (selectedObject != null)
        {
            PlaceObject();
        }
        else
        {
            DeselectObjectButton();
        }
    }
    private void PlaceObject()
    {
        GameObject spawnedObject = null;
        Renderer renderer = GetRenderer(selectedPrefab);
        if (selectedPrefab.GetPhotonView() != null)
        {
            spawnedObject = PhotonNetwork.Instantiate(selectedPrefab.name, new Vector3(mouseIndicator.transform.position.x, mouseIndicator.transform.position.y + renderer.bounds.size.y / 2, mouseIndicator.transform.position.z), Quaternion.identity);
        }
        else
        {
            spawnedObject = Instantiate(selectedPrefab, new Vector3(mouseIndicator.transform.position.x, mouseIndicator.transform.position.y + renderer.bounds.size.y / 2, mouseIndicator.transform.position.z), Quaternion.identity);
        }

        //Add Object to Hierarchy
        panelHierarchy.AddEntry(spawnedObject);

        Debug.Log("Intantiating Object: " + selectedObject.name);
                
        //Check if the Player is holding down CTRL. If they are, allow them to place multiple instances of the object
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            DeselectObjectButton();
        }
    }


    //When Clicking on an Interactable Object (Layer == Interactable), Select it
    private void OnClick_SelectPlacedObject() 
    {
        Vector3 mousePos = Input.mousePosition; //get the position of the mouse on the screen
        mousePos.z = sceneCamera.nearClipPlane; //Prevent Objects to be selected if they are not rendered by the camera

        //Raycast from mouse to game world filtered by placementMask
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, selectedLayermask))
        {
            SelectPlacedObject(hit.collider.gameObject);
        }
        else
        {
            DeselectPlacedObject();
        }
    }
    public void SelectPlacedObject(GameObject objectToSelect)
    {
        selectedPlacedObject = objectToSelect.gameObject;
        
        Debug.Log("Selected Object: " + selectedPlacedObject.name);
        
        //show pointer
        selectionPointer.SetActive(true);
    }
    
    
    //Re-usable Method for Deselecting a placed Object in the world
    private void DeselectPlacedObject()
    {
        if(selectedPlacedObject != null)
        {
            Debug.Log("Deselected Object: " + selectedPlacedObject.name);
            selectedPlacedObject = null;
        }
        //hide pointer
        selectionPointer.SetActive(false);
    }

    //Code that runs while a place object is selected
    private void SelectedPlacedObject()
    {
        if (selectedPlacedObject != null)
        {
            Renderer render = GetRenderer(selectedPlacedObject);
            

            selectionPointer.transform.position = new Vector3(selectedPlacedObject.transform.position.x, selectedPlacedObject.transform.position.y + render.bounds.size.y / 2, selectedPlacedObject.transform.position.z);

            //MoveObjectHorizontal
            if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject())
            {
                selectedPlacedObject.transform.position = new Vector3 (mouseIndicator.transform.position.x, selectedPlacedObject.transform.position.y, mouseIndicator.transform.position.z);
            }

            //MoveObjectVertical 
            if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftAlt) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Get the mouse position in screen coordinates
                Vector3 mouseScreenPosition = Input.mousePosition;

                // Convert the screen position to world position
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.WorldToScreenPoint(selectedPlacedObject.transform.position).z));

                // Set the object's Y position to the mouse's Y world position, keeping other coordinates the same
                selectedPlacedObject.transform.position = new Vector3(selectedPlacedObject.transform.position.x, mouseWorldPosition.y, selectedPlacedObject.transform.position.z);

            }

            //RotateObject
            if(Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Convert mouse position to world space, but we are only concerned with the X and Z coordinates
                Vector3 worldMousePosition = mouseIndicator.transform.position;

                // Calculate the direction vector on the XZ plane (ignoring Y for now)
                Vector3 direction = worldMousePosition - selectedPlacedObject.transform.position;

                // We only want to rotate on the Y axis, so we zero out the Y component
                direction.y = 0;

                // Calculate the Y-axis rotation
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    selectedPlacedObject.transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0); // Apply only Y-axis rotation
                }
            }

            //Delete The Object upon pressing Delete
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeletePlacedObject();
            }
        }
    }

    //Code that PhotonNetwork.Destroys a Selected, Placed Object
    public void DeletePlacedObject()
    {

        Debug.Log("Deleted Placed Object: " + selectedPlacedObject.name);
        
        //panelHierarchy.RemoveEntry(selectedPlacedObject);
        
        PhotonNetwork.Destroy(selectedPlacedObject);

        panelHierarchy.RemoveEntry();

        DeselectPlacedObject();

    }

    //Re-useable Method for removing current Object Selection
    private void DeselectObjectButton()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Button>().interactable = true;
            selectedObject = null;
        }
    }

    //Selects the Object based on the button clicked. Method called by the Button Script
    public void SelectObjectButton(GameObject selectedObject)
    {
        if(this.selectedObject != null)
        {
            this.selectedObject.GetComponent<Button>().interactable = true;
        }
        
        this.selectedObject = selectedObject;
        this.selectedObject.GetComponent<Button>().interactable = false;

        selectedPrefab = selectedObject.GetComponent<CreatorEntryBehaviour>().creatorEntryPrefab;

    }


    public void OnClick_OpenNPCEditor()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (selectedPlacedObject != null && selectedPlacedObject.GetComponent<NPCBehaviour>() != null)
            {
                npcEditor.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Unable to open NPC Editor, make sure to select a NPC");
            }
        }
    }


    Renderer GetRenderer(GameObject gameObject)
    {
        Renderer render;
        if (gameObject.GetComponent<Renderer>() != null)
        {
            render = gameObject.GetComponent<Renderer>();
            return render;
        }
        else if (gameObject.GetComponent<Renderer>() == null && selectedPlacedObject.GetComponentInChildren<Renderer>() != null)
        {
            render = gameObject.GetComponentInChildren<Renderer>();
            return render;
        }
        else
        {
            Debug.LogError("Asset has no renderer");
            return null;
        }
    }
}
