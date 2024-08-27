// using UnityEngine;
// using EzySlice;
// using Unity.VisualScripting;
// using System.Collections;
// using System;

// public class SlicingBehaviour : MonoBehaviour {
//     [SerializeField] Material _sliceMaterial;
//     [SerializeField] float _cutForce = 2500;
//     [SerializeField] VelocityEstimator _estimator;
//     [SerializeField] Transform _startSlicePoint, _endSlicePoint;
//     [SerializeField] ParticleSystem _electricityParticle;
//     [SerializeField] LayerMask _sliceLayer;
//     GameObject _scoreCanvas;
//     AudioSource[] audioSources;

//     //event for hand vibration
//     public delegate void HandsVibrating();
//     public static event HandsVibrating OnHandsVibrating;

//     void Start() {
//         _scoreCanvas = GameObject.Find("ScoreCanvas");
//         audioSources = GetComponents<AudioSource>();
//     }

//     private void Update()
//     {
//         bool hasHit = Physics.Raycast(_startSlicePoint.position, _endSlicePoint.position, out RaycastHit hit, _sliceLayer);

//         if (hasHit)
//         {
//             Debug.Log("HUACALA" + hit.collider.gameObject.name);
//             if (hit.collider.CompareTag("CorrectHitbox"))
//             {
//                 Debug.Log("HUACALA 1111" + hit.collider.gameObject.name);

//                 CorrectSlice(hit.collider.gameObject);
//             }
//             else if(hit.collider.CompareTag("Box"))
//             {

//                 Debug.Log("HUACALA 2222" + hit.collider.gameObject.name);

//                 WrongSlice(hit.collider.gameObject);
//             }
//         }
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         Debug.Log("COLLIDER" + other.gameObject.name);

//         if (other != null)
//         {
//             if (other.CompareTag("CorrectHitbox"))
//             {
//                 Debug.Log("COLLIDER11111" + other.gameObject.name);

//                 CorrectSlice(other.gameObject);
//             }
//             if (other.CompareTag("Box"))
//             {
//                 Debug.Log("COLLIDER2222" + other.gameObject.name);

//                 WrongSlice(other.gameObject);
//             }
//         }
//         OnHandsVibrating?.Invoke();
//     }

//     void OnCollisionEnter(Collision collision)
//     {
//         OnHandsVibrating?.Invoke();
//     }

//     void Slice(GameObject target) {
//         Vector3 velocity = _estimator.GetVelocityEstimate();
//         Vector3 planeNormal = Vector3.Cross(_endSlicePoint.position - _startSlicePoint.position, velocity);
//         planeNormal.Normalize();

//         SlicedHull hull = target.Slice(target.transform.position, planeNormal);
//         _electricityParticle.transform.position = target.transform.position;
//         _electricityParticle.Play();
//         if (hull == null) return;

//         var slicedHulls = CreateSlicedHulls(target, hull);

//         Destroy(target.transform.parent.gameObject);
//         StartCoroutine(DestroySlicedObjects(slicedHulls.Item1, slicedHulls.Item2));
//     }

//     Tuple<GameObject, GameObject> CreateSlicedHulls(GameObject target, SlicedHull hull) {
//         GameObject upperHull = hull.CreateUpperHull(target, _sliceMaterial);
//         GameObject lowerHull = hull.CreateLowerHull(target, _sliceMaterial);

//         upperHull.transform.position = _endSlicePoint.position;
//         lowerHull.transform.position = _endSlicePoint.position;

//         SetupSliceComponent(upperHull);
//         SetupSliceComponent(lowerHull);
//         return new Tuple<GameObject, GameObject>(upperHull, lowerHull);
//     }

//     void SetupSliceComponent(GameObject slicedObject) {
//         Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
//         MeshCollider collider = rb.AddComponent<MeshCollider>();
//         collider.convex = true;
//         rb.AddExplosionForce(_cutForce, slicedObject.transform.position, 1);
//         collider.isTrigger = true;
//     }

  

//     IEnumerator DestroySlicedObjects(GameObject upperHull, GameObject lowerHull) {
//         yield return new WaitForSeconds(2);
//         Destroy(upperHull);
//         Destroy(lowerHull);
//     }

//     public void CorrectSlice(GameObject other)
//     {
//         Transform body = other.transform.parent.Find("Body");
//         body.GetComponent<BoxCollider>().enabled = false;
//         _scoreCanvas.SendMessage("IncreaseScore");
//         Slice(body.gameObject);
//         audioSources[0].Play();
//     }

//     public void WrongSlice(GameObject other)
//     {
//         Transform correctHitbox = other.transform.parent.Find("CorrectHitbox");
//         correctHitbox.GetComponent<BoxCollider>().enabled = false;
//         _scoreCanvas.SendMessage("ResetCombo");
//         Slice(other.gameObject);
//         audioSources[1].Play();
//     }
// }