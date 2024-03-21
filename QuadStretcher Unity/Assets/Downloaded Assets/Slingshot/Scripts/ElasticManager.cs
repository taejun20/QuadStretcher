using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script will help to shorten or lengthen the size of the elastic when the MasterSlingshot's scale needs to be changed.
    For it to work well, check if all the bones are set on the inspector window.
    The script sets the parameters once when entering PlayMode.
*/

public class ElasticManager : MonoBehaviour
{
    [Range(1, 15)] //1 is the minimum number of bones for the elastic to work and 15 is the total number of bones in the FBX file.
    [SerializeField] private int numberOfBones;

    [Range(0, 7)] //elastic resistance to pull, this value will be caught in the script Slingshot.  
    public float elasticResistance;

    //The original values of SpringJoint components of the bones.
    public float spring = 3000f;
    public float minDistance = 0f;
    public float maxDistance = 0.15f;
    public float tolerance = 0.05f;
    public float elasticLinesWidth = 0.4f; //Controls the width of the line renderers.

    //Array of bone objects in the elastic.
    [SerializeField] private GameObject[] rightBones = new GameObject[15];
    [SerializeField] private GameObject[] leftBones = new GameObject[15];
    [SerializeField] private GameObject rightBoneEnd;
    [SerializeField] private GameObject leftBoneEnd;

    //Joints that are influenced by the tip of the elastic (last bone).
    [SerializeField] private GameObject leather;
    [SerializeField] private GameObject leatherLine;
    [SerializeField] private SpringJoint knotR;
    [SerializeField] private SpringJoint knotL;
    private FixedJoint[] leatherJoints;
    private FixedJoint[] leatherLineJoints;

    //LineRenderers used in the elastic effect when stretched.
    [SerializeField] private LineRenderer rightElasticLine;
    [SerializeField] private LineRenderer leftElasticLine;
    
    private int i = 1; //Used has index.
    
    void Awake()
    {
      
        //Sets the fixed joints that reference the last bone of the elastics
        leatherJoints = leather.GetComponents<FixedJoint>();
        leatherLineJoints = leatherLine.GetComponents<FixedJoint>();

        //Sets the width of the elastic lines when stretched.
        rightElasticLine.SetWidth(elasticLinesWidth, elasticLinesWidth);
        leftElasticLine.SetWidth(elasticLinesWidth, elasticLinesWidth);

        //Sets up each bone of the right elastic.
        foreach (GameObject bone in rightBones)
        {
            if(i <= numberOfBones)
            {
                //Sets the SpringJoint values to the used bones.
                SpringJoint bsj = bone.GetComponent<SpringJoint>();
                bsj.spring = spring;
                bsj.minDistance = minDistance;
                bsj.maxDistance = maxDistance;
                bsj.tolerance = tolerance;

                //Verify if it's the last bone.
                if (i == numberOfBones)
                {
                    //Sets the last bone according to the number of bones selected and links its rigidbody to the elastic and leather joints.
                    rightBoneEnd.transform.parent = bone.transform;
                    leatherJoints[1].connectedBody = bone.GetComponent<Rigidbody>();
                    leatherLineJoints[1].connectedBody = bone.GetComponent<Rigidbody>();
                    knotR.connectedBody = bone.GetComponent<Rigidbody>();
                }
            }
            else
            {
                //Disables the unused bones.
                bone.SetActive(false);
            }

            i++;
        }

        //Sets i to the minimun number of bones to repeat the procedure on the left elastic.
        i = 1;

        //Sets up each bone of the left elastic.
        foreach (GameObject bone in leftBones)
        {
            if (i <= numberOfBones)
            {
                //Sets the SpringJoint values to the used bones.
                SpringJoint bsj = bone.GetComponent<SpringJoint>();
                bsj.spring = spring;
                bsj.minDistance = minDistance;
                bsj.maxDistance = maxDistance;
                bsj.tolerance = tolerance;

                //Verify if it's the last bone.
                if (i == numberOfBones)
                {
                    //Sets the last bone according to the number of bones selected and links its rigidbody to the elastic and leather joints.
                    leftBoneEnd.transform.parent = bone.transform;
                    leatherJoints[0].connectedBody = bone.GetComponent<Rigidbody>();
                    leatherLineJoints[0].connectedBody = bone.GetComponent<Rigidbody>();
                    knotL.connectedBody = bone.GetComponent<Rigidbody>();
                }
            }
            else if (i > numberOfBones)
            {
                //Disables the unused bones.
                bone.SetActive(false);
            }

            i++;
        }
    }   
}
