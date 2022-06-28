using PsychoticLab;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CharacterEquipmentSelectorMode
{
    None = 0,
    LowerArms,
    UpperArms,
    Hands,
    ElbowPads,
    ShoulderPads,
    KneePads,
    Head,
    Torso,
    Hips,
    HipAttachment,
    Legs,
    BackAttachment,
    ChestAttachment,
    Gender
}

[ExecuteAfter(typeof(CharacterEquipmentController))]
public class CharacterEquipmentSelector : MonoBehaviour, IScrollHandler
{
    public bool isOver = false;

    [SerializeField] public CharacterEquipmentController characterEquipmentController;
    [SerializeField] public EventSystem eventSystem;
    public CharacterEquipmentSelectorMode Mode;

    public void Awake()
    {
        switch (Mode)
        {
            case CharacterEquipmentSelectorMode.None:
                break;
            case CharacterEquipmentSelectorMode.LowerArms:
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.arm_Lower_Left, GetChildByName("MaleLeftParts"));
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.arm_Lower_Right, GetChildByName("MaleRightParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.arm_Lower_Left, GetChildByName("FemaleLeftParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.arm_Lower_Right, GetChildByName("FemaleRightParts"));
                break;
            case CharacterEquipmentSelectorMode.UpperArms:
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.arm_Upper_Left, GetChildByName("MaleLeftParts"));
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.arm_Upper_Right, GetChildByName("MaleRightParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.arm_Upper_Left, GetChildByName("FemaleLeftParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.arm_Upper_Right, GetChildByName("FemaleRightParts"));
                break;
            case CharacterEquipmentSelectorMode.Hands:
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.hand_Left, GetChildByName("MalePartsLeft"));
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.hand_Right, GetChildByName("MalePartsRight"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.hand_Left, GetChildByName("FemalePartsLeft"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.hand_Right, GetChildByName("FemalePartsRight"));
                break;
            case CharacterEquipmentSelectorMode.ElbowPads:
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.elbow_Attachment_Left, GetChildByName("PartsLeft"));
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.elbow_Attachment_Right, GetChildByName("PartsRight"));
                break;
            case CharacterEquipmentSelectorMode.ShoulderPads:
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.shoulder_Attachment_Left, GetChildByName("PartsLeft"));
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.shoulder_Attachment_Right, GetChildByName("PartsRight"));
                break;
            case CharacterEquipmentSelectorMode.KneePads:
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.knee_Attachement_Left, GetChildByName("PartsLeft"));
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.knee_Attachement_Right, GetChildByName("PartsRight"));
                break;
            case CharacterEquipmentSelectorMode.Head:
                break;
            case CharacterEquipmentSelectorMode.Torso:
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.torso, GetChildByName("MaleParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.torso, GetChildByName("FemaleParts"));
                break;
            case CharacterEquipmentSelectorMode.Hips:
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.hips, GetChildByName("MaleParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.hips, GetChildByName("FemaleParts"));
                break;
            case CharacterEquipmentSelectorMode.HipAttachment:
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.hips_Attachment, GetChildByName("Parts"));
                break;
            case CharacterEquipmentSelectorMode.Legs:
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.leg_Left, GetChildByName("MaleLeftParts"));
                characterEquipmentController.BuildList(characterEquipmentController.MaleStatic.leg_Right, GetChildByName("MaleRightParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.leg_Left, GetChildByName("FemaleLeftParts"));
                characterEquipmentController.BuildList(characterEquipmentController.FemaleStatic.leg_Right, GetChildByName("FemaleRightParts"));
                break;
            case CharacterEquipmentSelectorMode.BackAttachment:
                characterEquipmentController.BuildList(characterEquipmentController.AllGenderStatic.back_Attachment, GetChildByName("Parts"));
                break;
            case CharacterEquipmentSelectorMode.ChestAttachment:
                break;
            case CharacterEquipmentSelectorMode.Gender:
                break;
        }
    }

    private Transform GetChildByName(string characterPart)
    {
        Transform[] rootTransform = gameObject.GetComponentsInChildren<Transform>();

        // declare target root transform
        Transform targetRoot = null;

        // find character parts parent object in the scene
        foreach (Transform t in rootTransform)
        {
            if (t.gameObject.name == characterPart)
            {
                targetRoot = t;
                break;
            }
        }

        return targetRoot;
    }


    public void OnScroll(PointerEventData eventData)
    {
        if(eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        int value = eventData.scrollDelta.y > 0 ? +1 : -1;

        switch (Mode)
        {
            case CharacterEquipmentSelectorMode.None:
                break;
            case CharacterEquipmentSelectorMode.LowerArms:
                characterEquipmentController.LowerArms++;
                break;
            case CharacterEquipmentSelectorMode.UpperArms:
                characterEquipmentController.UpperArms++;
                break;
            case CharacterEquipmentSelectorMode.Hands:
                characterEquipmentController.Hands++;
                break;
            case CharacterEquipmentSelectorMode.ElbowPads:
                characterEquipmentController.ElbowAttachment++;
                break;
            case CharacterEquipmentSelectorMode.ShoulderPads:
                characterEquipmentController.ShoulderAttachment++;
                break;
            case CharacterEquipmentSelectorMode.KneePads:
                characterEquipmentController.KneeAttachement++;
                break;
            case CharacterEquipmentSelectorMode.Head:
                break;
            case CharacterEquipmentSelectorMode.Torso:
                characterEquipmentController.Torso++;
                break;
            case CharacterEquipmentSelectorMode.Hips:
                characterEquipmentController.Hips++;
                break;
            case CharacterEquipmentSelectorMode.HipAttachment:
                characterEquipmentController.HipsAttachment++;
                break;
            case CharacterEquipmentSelectorMode.Legs:
                characterEquipmentController.Legs++;
                break;
            case CharacterEquipmentSelectorMode.BackAttachment:
                characterEquipmentController.BackAttachment++;
                break;
            case CharacterEquipmentSelectorMode.ChestAttachment:
                characterEquipmentController.ChestAttachment++;
                break;
            case CharacterEquipmentSelectorMode.Gender:
                var text = gameObject.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    characterEquipmentController.Gender++;
                    text.text = characterEquipmentController.Gender.ToString().ToUpper();
                }
                break;
        }

        characterEquipmentController.RenderCharacter();
    }
}