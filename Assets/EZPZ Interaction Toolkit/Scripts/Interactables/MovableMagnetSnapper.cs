//EZPZ Interaction Toolkit
//by Matt Cabanag
//created 28 Nov Jun 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovableMagnetSnapper : MonoBehaviour
{
    [Header("Snap Settings")]
    [Tooltip("Where objects will get snapped to the magnet snapper")]
    public Transform snappingPoint;
    [Tooltip("Align snapped object to magnet snapper's rotation")]
    public bool alignRotation = true;
    [Tooltip("Use to include objects. Leave empty to accept all items")]
    public string filterString;
    [Tooltip("Use this to exclude objects. Leave empty to accept all items")]
    public string excludFilterString;
    [Tooltip("Keep the snap area visible at runtime")]
    public bool visibleAtRuntime = true;

    [Header("Event Handling")]
    public UnityEvent onSnap;
    public UnityEvent onRelease;
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTriggerStay;


    [Header("System Stuff (Usually Don't Touch)")]
    public Movable subject;
    public bool snapFlag = true;    
    public Vector3 subjectLocalAttachPos;
    public Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (snappingPoint == null)
            snappingPoint = transform;

        snapFlag = true;

        if (snappingPoint.parent.localScale != Vector3.one || snappingPoint.localScale != Vector3.one)
        {
            Debug.LogError("SNAPPING POINT SCALE MISMATCH " + name + " snapping point scale or its parent is not (1,1,1)");
        }

        if (myRenderer == null)
        {
            myRenderer = GetComponent<Renderer>();
        }

        if(myRenderer != null)
            myRenderer.enabled = visibleAtRuntime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(subject != null)
        {
            HandleSnapping();
            HandleFixedPos();
        }
    }

    public void HandleSnapping()
    {
        // 只有在 snapFlag 被置为 false（OnTriggerEnter 通过过滤之后）
        // 并且 subject 不在移动时才吸附
        if (!snapFlag)
        {
            if (subject != null && !subject.moving && subject.myMagnetSnapper == null)
            {
                subject.myMagnetSnapper = this;

                // ---- 核心：强制把木棍绑到 snappingPoint 上 ----
                // 先作为子物体
                subject.transform.SetParent(snappingPoint);

                // 把局部位置归零，相当于世界坐标 = snappingPoint 的位置
                subject.transform.localPosition = Vector3.zero;

                // 对齐旋转（如果你勾选了 alignRotation）
                if (alignRotation)
                {
                    subject.transform.rotation = snappingPoint.rotation;
                }

                // 记录当前局部位置，后面 HandleFixedPos 会持续锁定在这里
                subjectLocalAttachPos = subject.transform.localPosition;

                Debug.Log("On Snap (forced to snappingPoint)!");
                onSnap.Invoke();

                // 停止物理运动
                if (subject.myRbody != null)
                {
                    subject.myRbody.linearVelocity = Vector3.zero;
                    subject.myRbody.useGravity = false;
                }

                // 标记已经吸附完成
                snapFlag = true;
            }
        }
    }

    public void HandleFixedPos()
    {
        if (!subject.moving)
        {
            if (subject.transform.parent == snappingPoint)
            {
                if (snapFlag)
                {
                    if (subject.myRbody != null)
                        subject.myRbody.linearVelocity = Vector3.zero;

                    subject.transform.localPosition = subjectLocalAttachPos;
                    subject.transform.rotation = snappingPoint.rotation;
                }
            }
            else
            {
                subject = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (subject == null)
        {
            subject = other.GetComponent<Movable>();

            if (subject != null)
            {
                if (subject.myMagnetSnapper == null)
                {
                    if (filterString.Length > 0 || excludFilterString.Length > 0)
                    {
                        TriggerFilter tf = subject.GetComponent<TriggerFilter>();


                        if (excludFilterString.Length > 0)
                        {
                            //Debug.Log("MovableMagnetSnapper: exculdeFilter: " + excludFilterString);
                            if (tf != null)
                            {
                                if (tf.filterString.Equals(excludFilterString))
                                {
                                    subject = null;                                    
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (filterString.Length > 0)
                            {
                                if (tf != null)
                                {
                                    if (!tf.filterString.Equals(filterString))
                                    {
                                        subject = null;
                                        return;
                                    }
                                }
                                else
                                {
                                    subject = null;
                                    snapFlag = false;
                                    return;
                                }
                            }
                        }
                    }

                    if (subject.myMagnetSnapper == null)
                    {
                        snapFlag = false;
                    }
                    else
                    {
                        subject = null;
                        return;
                    }
                }
            }
        }

        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (subject != null)
        {
            Movable om = other.GetComponent<Movable>();

            if (om != null)
            {
                //make sure the object exiting the area
                //is exactly the one that's leaving...
                if (om == subject)
                {
                    if (subject.moving)
                    {
                        SoftReleaseSubject();
                    }
                }
                //don't want another object to trigger dropping
            }
        }

        onTriggerExit.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke();
    }

    public void ReleaseSubject()
    {
        if(subject != null)
        {
            Debug.Log("ReleaseSubject");
            Rigidbody r = subject.GetComponent<Rigidbody>();           

            if (r != null)
            {
                r.useGravity = true;
                r.linearVelocity = Vector3.zero;                
                r.isKinematic = false;
            }

            SoftReleaseSubject();
        }
    }

    public void SoftReleaseSubject()
    {
        onRelease.Invoke();

        subjectLocalAttachPos = Vector3.zero;
        subject.myMagnetSnapper = null;
        snapFlag = true;
        subject = null;
    }
    
}
