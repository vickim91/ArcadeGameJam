using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioLoop), true)]
public class AudioLoopEditor : Editor
{

	//[SerializeField] private AudioSource _previewer;

	//public void OnEnable()
	//{
	//	_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	//}

	//public void OnDisable()
	//{
	//	DestroyImmediate(_previewer.gameObject);
	//}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Trigger Seek And Play"))
		{
			((AudioLoop)target).TriggerSeekAndPlay();
		}
		if (GUILayout.Button("Stop Audio Loop"))
		{
			((AudioLoop)target).StopAudioLoop();
		}
		EditorGUI.EndDisabledGroup();
	}

}
