using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public Transform m_Focus;

    [SerializeField] public Vector3 m_offset = new Vector3(10, 10, 10);

    [SerializeField] public CircuitController m_Circuit;
    [SerializeField] private float m_Distance = 10;
    [SerializeField] private float m_Elevation = 8;
    [Range(0, 1)] [SerializeField] private float m_Following = 0.5f;

    private Vector3 m_Direction = Vector3.zero;

    private Vector3 previousLookAt;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = this.GetComponent<Camera>();

        previousLookAt = mainCamera.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Focus != null)
        {
            if (this.m_Circuit != null)
            {
                if (this.m_Direction.magnitude == 0)
                {
                    this.m_Direction = new Vector3(0f, -1f, 0f);
                }

                int segIdx;
                float carDist;
                Vector3 carProj;

                m_Circuit.ComputeClosestPointArcLength(m_Focus.position, out segIdx, out carProj,
                    out carDist );

                Vector3 pathDir = -m_Circuit.GetSegment(segIdx);
                pathDir = new Vector3(pathDir.x, 0f, pathDir.z);
                pathDir.Normalize();

                this.m_Direction = Vector3.Lerp(this.m_Direction, pathDir, this.m_Following * Time.deltaTime);
                Vector3 offset = this.m_Direction * this.m_Distance;
                offset = new Vector3(offset.x, m_Elevation, offset.z);

                mainCamera.transform.position = m_Focus.position + offset;
                mainCamera.transform.LookAt(m_Focus.position);
            }
            else
            {
                mainCamera.transform.position = m_Focus.position + m_offset;
                mainCamera.transform.LookAt(m_Focus.position);

            }
        }
    }


}