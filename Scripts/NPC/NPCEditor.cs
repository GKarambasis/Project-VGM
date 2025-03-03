using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPCEditor : MonoBehaviour
{
    //NPC Attribute References
    [SerializeField] TMP_InputField npc_Name_field;
    [SerializeField] TMP_InputField npc_Race_field;
    [SerializeField] TMP_InputField npc_Gender_field;
    [SerializeField] TMP_InputField npc_Occupation_field;
    [SerializeField] TMP_InputField npc_Personality_field;
    [SerializeField] TMP_InputField npc_Voice_field;
    [SerializeField] TMP_InputField npc_CurrentObjective_field;
    [SerializeField] TMP_InputField npc_PlayerInteractionDetails_field;

    [SerializeField] Toggle toggle;

    TMP_InputField[] npc_Attributes_field = new TMP_InputField[8];

    //Reference to the VGM Controller to get the selected npc
    private VGMInputController vgmController;
    //reference to the npcs behaviour and attack state
    private NPCBehaviour selectedNPCBehaviour;
    private NPCAttack selectedNPCAttack;

    private void Awake()
    {
        npc_Attributes_field[0] = npc_Name_field;
        npc_Attributes_field[1] = npc_Race_field;
        npc_Attributes_field[2] = npc_Gender_field;
        npc_Attributes_field[3] = npc_Occupation_field;
        npc_Attributes_field[4] = npc_Personality_field;
        npc_Attributes_field[5] = npc_Voice_field;
        npc_Attributes_field[6] = npc_CurrentObjective_field;
        npc_Attributes_field[7] = npc_PlayerInteractionDetails_field;


        vgmController = FindAnyObjectByType<VGMInputController>();
        
    }

    private void OnEnable()
    {
        //Check to see if the object is an npc (it should be but check anyhow)
        //Get the selected object from the VGM controller
        selectedNPCBehaviour = vgmController.selectedPlacedObject.GetComponent<NPCBehaviour>();

        selectedNPCAttack = vgmController.selectedPlacedObject.GetComponent<NPCAttack>();

        //Set the Text of the input fields to the information of the npc 
        for (int i = 0; i < npc_Attributes_field.Length; i++)
        {
            npc_Attributes_field[i].text = selectedNPCBehaviour.npc_Attributes[i];
        }

        toggle.isOn = selectedNPCAttack.isActive;
    }

    public void OnClick_SaveAttributes()
    {
        //Called when clicking the Save Button to save the attributes of the NPC
        selectedNPCBehaviour.UpdateAttributes(npc_Attributes_field);

        selectedNPCAttack.AttackState(toggle.isOn);
    }
    

}
