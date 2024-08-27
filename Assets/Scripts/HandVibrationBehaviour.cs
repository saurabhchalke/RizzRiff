// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class HandVibrationBehaviour : MonoBehaviour {

//     [SerializeField] XRBaseController leftController, rightController;

//     //void OnEnable() => SlicingBehaviour.OnHandsVibrating += SendHaptics;
//     //void OnDisable() => SlicingBehaviour.OnHandsVibrating -= SendHaptics;

//     void Update() {
//         SendHaptics();
//     }

//     void SendHaptics() {

//         leftController.SendHapticImpulse(0.5f, 0.1f);
//         rightController.SendHapticImpulse(0.5f, 0.1f);
//     }
// }
