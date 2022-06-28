// based on character randomizer version 1.30
using System.Collections.Generic;
using UnityEngine;

namespace PsychoticLab
{
    public class CharacterEquipmentController : MonoBehaviour
    {
        [Header("Material")]
        public Material mat;

        // list of enabed objects on character
        [HideInInspector]
        public List<GameObject> enabledObjects = new List<GameObject>();

        // character object lists
        // male list
        [HideInInspector]
        public CharacterObjectGroups Male;
        // female list
        [HideInInspector]
        public CharacterObjectGroups Female;
        // universal list
        [HideInInspector]
        public CharacterObjectListsAllGender AllGender;

        // reference to camera transform, used for rotation around the model during or after a randomization (this is sourced from Camera.main, so the main camera must be in the scene for this to work)
        Transform camHolder;

        // cam rotation x
        float x = 16;

        // cam rotation y
        float y = -30;



        private void Start()
        {
            // rebuild all lists
            BuildLists();

            // setting up the camera position, rotation, and reference for use
            Transform cam = Camera.main.transform;
            if (cam)
            {
                cam.position = transform.position + new Vector3(0, 0.3f, 2);
                cam.rotation = Quaternion.Euler(0, -180, 0);
                camHolder = new GameObject().transform;
                camHolder.position = transform.position + new Vector3(0, 1, 0);
                cam.LookAt(camHolder);
                cam.SetParent(camHolder);
                y = Mathf.Clamp(y, -45, 15);
                camHolder.eulerAngles = new Vector3(y, x, 0.0f);
            }


            RenderCharacter();
        }




        private Gender gender = 0;
        private int torso = 0;
        private int armUpper = 0;
        private int armLower = 0;
        private int hand = 0;
        private int hips = 0;
        private int leg = 0;

        private int chestAttachment = 0;
        private int backAttachment = 0;
        private int hipsAttachment = 0;
        private int shoulderAttachment = 0;
        private int elbowAttachment = 0;
        private int kneeAttachement = 0;



        public Gender Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = (Gender)((int)value % 2);

                //Update uneven gender parts
                //Torso = Torso;
                //UpperArms = UpperArms;
                //LowerArms = LowerArms;
                //Hands = Hands;
                //Hips = Hips;
                //Legs = Legs;
            }
        }
        public CharacterObjectGroups CurrentGenderedCharacter => (this.Gender) switch
        {
            Gender.Female => this.Female,
            Gender.Male => this.Male,
            _ => this.Male
        };
        public CharacterObjectGroups CurrentGenderedCharacterStatic => (this.Gender) switch
        {
            Gender.Female => this.FemaleStatic,
            Gender.Male => this.MaleStatic,
            _ => this.MaleStatic
        };
        public int Torso
        {
            get { return torso; }
            set { torso = value % CurrentGenderedCharacter.torso.Count; }
        }
        public int UpperArms
        {
            get { return armUpper; }
            set { armUpper = value % Mathf.Min(CurrentGenderedCharacter.arm_Upper_Right.Count, CurrentGenderedCharacter.arm_Upper_Left.Count); }
        }
        public int LowerArms
        {
            get { return armLower; }
            set { armLower = value % Mathf.Min(CurrentGenderedCharacter.arm_Lower_Right.Count, CurrentGenderedCharacter.arm_Lower_Left.Count); }
        }
        public int Hands
        {
            get { return hand; }
            set { hand = value % Mathf.Min(CurrentGenderedCharacter.hand_Right.Count, CurrentGenderedCharacter.hand_Left.Count); }
        }
        public int Hips
        {
            get { return hips; }
            set { hips = value % CurrentGenderedCharacter.hips.Count; }
        }
        public int Legs
        {
            get { return leg; }
            set { leg = value % Mathf.Min(CurrentGenderedCharacter.leg_Right.Count, CurrentGenderedCharacter.leg_Left.Count); }
        }
        public int ChestAttachment
        {
            get { return chestAttachment; }
            set { chestAttachment = value % (AllGender.chest_Attachment.Count + 1); }
        }
        public int BackAttachment
        {
            get { return backAttachment; }
            set { backAttachment = value % (AllGender.back_Attachment.Count + 1); }
        }
        public int HipsAttachment
        {
            get { return hipsAttachment; }
            set { hipsAttachment = value % (AllGender.hips_Attachment.Count + 1); }
        }
        public int ShoulderAttachment
        {
            get { return shoulderAttachment; }
            set { shoulderAttachment = value % (Mathf.Min(AllGender.shoulder_Attachment_Right.Count, AllGender.shoulder_Attachment_Left.Count) + 1); }
        }
        public int ElbowAttachment
        {
            get { return elbowAttachment; }
            set { elbowAttachment = value % (Mathf.Min(AllGender.elbow_Attachment_Right.Count, AllGender.elbow_Attachment_Left.Count) + 1); }
        }
        public int KneeAttachement
        {
            get { return kneeAttachement; }
            set { kneeAttachement = value % (Mathf.Min(AllGender.knee_Attachement_Right.Count, AllGender.knee_Attachement_Left.Count) + 1); }
        }

        // character object lists
        // male list
        [HideInInspector]
        public CharacterObjectGroups MaleStatic = new CharacterObjectGroups();
        // female list
        [HideInInspector]
        public CharacterObjectGroups FemaleStatic = new CharacterObjectGroups();
        // universal list
        [HideInInspector]
        public CharacterObjectListsAllGender AllGenderStatic = new CharacterObjectListsAllGender();





        public void RenderCharacter()
        {

            foreach (GameObject g in enabledObjects)
                g.SetActive(false);
            enabledObjects.Clear();

            ActivateItem(CurrentGenderedCharacter.headAllElements[0]);

            ActivateItem(CurrentGenderedCharacter.torso[torso]);
            ActivateItem(CurrentGenderedCharacterStatic.torso[torso]);

            ActivateItem(CurrentGenderedCharacter.arm_Upper_Right[armUpper]);
            ActivateItem(CurrentGenderedCharacter.arm_Upper_Left[armUpper]);
            ActivateItem(CurrentGenderedCharacter.arm_Lower_Right[armLower]);
            ActivateItem(CurrentGenderedCharacter.arm_Lower_Left[armLower]);
            ActivateItem(CurrentGenderedCharacterStatic.arm_Upper_Right[armUpper]);
            ActivateItem(CurrentGenderedCharacterStatic.arm_Upper_Left[armUpper]);
            ActivateItem(CurrentGenderedCharacterStatic.arm_Lower_Right[armLower]);
            ActivateItem(CurrentGenderedCharacterStatic.arm_Lower_Left[armLower]);

            ActivateItem(CurrentGenderedCharacter.hand_Right[hand]);
            ActivateItem(CurrentGenderedCharacter.hand_Left[hand]);
            ActivateItem(CurrentGenderedCharacterStatic.hand_Right[hand]);
            ActivateItem(CurrentGenderedCharacterStatic.hand_Left[hand]);
            ActivateItem(CurrentGenderedCharacter.hips[hips]);
            ActivateItem(CurrentGenderedCharacterStatic.hips[hips]);

            ActivateItem(CurrentGenderedCharacter.leg_Right[leg]);
            ActivateItem(CurrentGenderedCharacter.leg_Left[leg]);
            ActivateItem(CurrentGenderedCharacterStatic.leg_Right[leg]);
            ActivateItem(CurrentGenderedCharacterStatic.leg_Left[leg]);

            if (chestAttachment > 0)
                ActivateItem(AllGender.chest_Attachment[chestAttachment - 1]);
            if (backAttachment > 0)
            {
                ActivateItem(AllGender.back_Attachment[backAttachment - 1]);
                ActivateItem(AllGenderStatic.back_Attachment[backAttachment - 1]);
            }
            if (hipsAttachment > 0)
            {
                ActivateItem(AllGender.hips_Attachment[hipsAttachment - 1]);
                ActivateItem(AllGenderStatic.hips_Attachment[hipsAttachment - 1]);
            }
            if (shoulderAttachment > 0)
            {
                ActivateItem(AllGender.shoulder_Attachment_Right[shoulderAttachment - 1]);
                ActivateItem(AllGender.shoulder_Attachment_Left[shoulderAttachment - 1]);
                ActivateItem(AllGenderStatic.shoulder_Attachment_Right[shoulderAttachment - 1]);
                ActivateItem(AllGenderStatic.shoulder_Attachment_Left[shoulderAttachment - 1]);
            }
            if (elbowAttachment > 0)
            {
                ActivateItem(AllGender.elbow_Attachment_Left[elbowAttachment - 1]);
                ActivateItem(AllGender.elbow_Attachment_Right[elbowAttachment - 1]);
                ActivateItem(AllGenderStatic.elbow_Attachment_Left[elbowAttachment - 1]);
                ActivateItem(AllGenderStatic.elbow_Attachment_Right[elbowAttachment - 1]);
            }
            if (kneeAttachement > 0)
            {
                ActivateItem(AllGender.knee_Attachement_Left[kneeAttachement - 1]);
                ActivateItem(AllGender.knee_Attachement_Right[kneeAttachement - 1]);
                ActivateItem(AllGenderStatic.knee_Attachement_Left[kneeAttachement - 1]);
                ActivateItem(AllGenderStatic.knee_Attachement_Right[kneeAttachement - 1]);
            }

        }

        // enable game object and add it to the enabled objects list
        void ActivateItem(GameObject go)
        {
            // enable item
            go.SetActive(true);

            // add item to the enabled items list
            enabledObjects.Add(go);
        }

        // build all item lists for use in randomization
        private void BuildLists()
        {
            //build out male lists
            BuildList(Male.headAllElements, "Male_Head_All_Elements");
            BuildList(Male.headNoElements, "Male_Head_No_Elements");
            BuildList(Male.eyebrow, "Male_01_Eyebrows");
            BuildList(Male.facialHair, "Male_02_FacialHair");
            BuildList(Male.torso, "Male_03_Torso");
            BuildList(Male.arm_Upper_Right, "Male_04_Arm_Upper_Right");
            BuildList(Male.arm_Upper_Left, "Male_05_Arm_Upper_Left");
            BuildList(Male.arm_Lower_Right, "Male_06_Arm_Lower_Right");
            BuildList(Male.arm_Lower_Left, "Male_07_Arm_Lower_Left");
            BuildList(Male.hand_Right, "Male_08_Hand_Right");
            BuildList(Male.hand_Left, "Male_09_Hand_Left");
            BuildList(Male.hips, "Male_10_Hips");
            BuildList(Male.leg_Right, "Male_11_Leg_Right");
            BuildList(Male.leg_Left, "Male_12_Leg_Left");

            //build out female lists
            BuildList(Female.headAllElements, "Female_Head_All_Elements");
            BuildList(Female.headNoElements, "Female_Head_No_Elements");
            BuildList(Female.eyebrow, "Female_01_Eyebrows");
            BuildList(Female.facialHair, "Female_02_FacialHair");
            BuildList(Female.torso, "Female_03_Torso");
            BuildList(Female.arm_Upper_Right, "Female_04_Arm_Upper_Right");
            BuildList(Female.arm_Upper_Left, "Female_05_Arm_Upper_Left");
            BuildList(Female.arm_Lower_Right, "Female_06_Arm_Lower_Right");
            BuildList(Female.arm_Lower_Left, "Female_07_Arm_Lower_Left");
            BuildList(Female.hand_Right, "Female_08_Hand_Right");
            BuildList(Female.hand_Left, "Female_09_Hand_Left");
            BuildList(Female.hips, "Female_10_Hips");
            BuildList(Female.leg_Right, "Female_11_Leg_Right");
            BuildList(Female.leg_Left, "Female_12_Leg_Left");

            // build out all gender lists
            BuildList(AllGender.all_Hair, "All_01_Hair");
            BuildList(AllGender.all_Head_Attachment, "All_02_Head_Attachment");
            BuildList(AllGender.headCoverings_Base_Hair, "HeadCoverings_Base_Hair");
            BuildList(AllGender.headCoverings_No_FacialHair, "HeadCoverings_No_FacialHair");
            BuildList(AllGender.headCoverings_No_Hair, "HeadCoverings_No_Hair");
            BuildList(AllGender.chest_Attachment, "All_03_Chest_Attachment");
            BuildList(AllGender.back_Attachment, "All_04_Back_Attachment");
            BuildList(AllGender.shoulder_Attachment_Right, "All_05_Shoulder_Attachment_Right");
            BuildList(AllGender.shoulder_Attachment_Left, "All_06_Shoulder_Attachment_Left");
            BuildList(AllGender.elbow_Attachment_Right, "All_07_Elbow_Attachment_Right");
            BuildList(AllGender.elbow_Attachment_Left, "All_08_Elbow_Attachment_Left");
            BuildList(AllGender.hips_Attachment, "All_09_Hips_Attachment");
            BuildList(AllGender.knee_Attachement_Right, "All_10_Knee_Attachement_Right");
            BuildList(AllGender.knee_Attachement_Left, "All_11_Knee_Attachement_Left");
            BuildList(AllGender.elf_Ear, "Elf_Ear");
        }

        // called from the BuildLists method
        void BuildList(List<GameObject> targetList, string characterPart)
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

            BuildList(targetList, targetRoot);
        }


        public void BuildList(List<GameObject> targetList, Transform targetRoot)
        {
            // clears targeted list of all objects
            targetList.Clear();

            // cycle through all child objects of the parent object
            for (int i = 0; i < targetRoot.childCount; i++)
            {
                // get child gameobject index i
                GameObject go = targetRoot.GetChild(i).gameObject;

                // disable child object
                go.SetActive(false);

                // add object to the targeted object list
                targetList.Add(go);
            }
        }
    }
}
