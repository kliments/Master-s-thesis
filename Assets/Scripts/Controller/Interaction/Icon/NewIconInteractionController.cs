using System;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Controller.Interaction.Icon
{
    public class NewIconInteractionController : GenericIconInteractionController
    {
        public static GenericOperator ClickedOp;
        private List<GenericIcon> optionIcons;
        private List<GenericIcon> optionIconPrefabs;
        private List<GameObject> operatorPrefabs;
        private bool optionsDisplayed = false;

        protected override void OnLeftClickOnTargetEventAction()
        {
            Debug.Log("New Operator Clicked");

            ClickedOp = GetOperator();

            if (optionsDisplayed)
            {
                hideOptions();
            }
            else
            {
                displayOptions();
            }
        }

        private List<GameObject> lines;

        public void displayOptions()
        {
            optionIconPrefabs = new List<GenericIcon>();
            lines = new List<GameObject>();
            var prefabs = GetOperator().Observer.GetOperatorPrefabs();
            operatorPrefabs = new List<GameObject>();

            foreach (GameObject prefab in prefabs)
            {
                //first validate if the respective prefab can be used with the current parent
                if (prefab.GetComponent<GenericOperator>().ValidateIfOperatorPossibleForParents(ClickedOp))
                {
                    optionIconPrefabs.Add(prefab.GetComponent<GenericOperator>().GetComponentInChildren<GenericIcon>());
                    operatorPrefabs.Add(prefab);
                }
            }

            optionIcons = new List<GenericIcon>();
            foreach (GenericIcon optionIcon in optionIconPrefabs)
            {
                GenericIcon instance = GameObject.Instantiate(optionIcon);
                Targetable[] scripts = instance.GetComponentsInChildren<Targetable>();
                foreach (Targetable script in scripts)
                {
                    Destroy(script);
                }

                instance.transform.parent = ClickedOp.GetIcon().transform.parent;
                instance.transform.localScale = ClickedOp.GetIcon().transform.localScale;
                instance.transform.position = getSpawnPositionOffsetForButton(ClickedOp.GetIcon().transform, optionIcons.Count, optionIconPrefabs.Count);

                foreach (Transform child in instance.gameObject.GetComponentsInChildren<Transform>())
                {
                    child.gameObject.AddComponent<NewIconInteractionButton>();
                    child.gameObject.GetComponent<NewIconInteractionButton>().controller = this;
                    child.gameObject.GetComponent<NewIconInteractionButton>().id = optionIcons.Count;
                }
                
                optionIcons.Add(instance);

                // add lines between the icons 
                GameObject line = new GameObject();
                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                lineRenderer.SetWidth(0.01f, 0.01f);
                lineRenderer.SetPositions(new Vector3[] {
                    GetOperator().GetIcon().transform.position+new Vector3(0,0,0.001f), instance.transform.position+new Vector3(0,0,0.001f)
                });

                lines.Add(line);
            }
            optionsDisplayed = true;

        }

        public void hideOptions()
        {
            foreach (GenericIcon instance in optionIcons)
            {
                GameObject.Destroy(instance.gameObject);
            }
            foreach (GameObject line in lines)
            {
                GameObject.Destroy(line);
            }
            optionIcons = new List<GenericIcon>();
            optionsDisplayed = false;
        }

        private Vector3 getSpawnPositionOffsetForButton(Transform origin, int nrButton, int totalButtons)
        {
            var defaultDistance = 1;

            

            Quaternion rotationSave = origin.rotation;
            Vector3 newPos;

            if (totalButtons == 1)
            {
                origin.Rotate(new Vector3(0, 90, 0));
                newPos = origin.position + origin.forward * defaultDistance;
                origin.rotation = rotationSave;
            }
            else
            {
                float maxangle = Math.Min(300, totalButtons * 10);
                float step = maxangle / (totalButtons - 1);
                float startdegree = maxangle / 2;

                float destinationdegree = nrButton * step;
                origin.Rotate(new Vector3(-startdegree + destinationdegree, 90, 0));
                newPos = origin.position + origin.forward * defaultDistance;
            }
            origin.rotation = rotationSave;


            return newPos;
        }

        public void spawnPrefab(int id)
        {
            GameObject spawnedPrefab = GetOperator().Observer.CreateOperator(operatorPrefabs[id], GetOperator().Parents);
      
            hideOptions();
            
            GetOperator().Observer.NewOperatorInitializedAndRunnningEvent += moveNewIconAndDestroyOperator;
        }

        public void moveNewIconAndDestroyOperator(GenericOperator genericOperator)
        {
            genericOperator.GetIcon().gameObject.transform.localPosition = GetOperator().GetIcon().transform.localPosition;

            GetOperator().Observer.NewOperatorInitializedAndRunnningEvent -= moveNewIconAndDestroyOperator;
            GetOperator().Observer.DestroyOperator(GetOperator());
        }

      
        
    }
}
