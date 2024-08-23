using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Wood))]
public class WoodEditor : Editor
{
    SerializedProperty Size;

    private Transform _transform;


    private void OnEnable()
    {
        Size = serializedObject.FindProperty("Size");
        _transform = (target as Wood).transform;
    }

    public override void OnInspectorGUI()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            DrawDefaultInspector();
        }
    }

    public void OnSceneGUI()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            EditorGUI.BeginChangeCheck();

            _transform.position = new Vector3(_transform.position.x, _transform.position.y, 0);
            _transform.rotation = Quaternion.identity;
            _transform.localScale = Vector3.one;

            Handles.color = Color.green;
            var fmh_41_21_638594974454974186 = Quaternion.identity; Size.vector2Value = Handles.FreeMoveHandle(
                    _transform.position + (Vector3)Size.vector2Value,
                    HandleUtility.GetHandleSize(_transform.position + (Vector3)Size.vector2Value) / 5,
                    Vector3.one * 0.05f,
                    Handles.CubeHandleCap)
                - _transform.position;
            if (Size.vector2Value.x < 1 || Size.vector2Value.y < 1)
            {
                Size.vector2Value = new Vector2(Size.vector2Value.x < 1 ? 1 : Size.vector2Value.x, Size.vector2Value.y < 1 ? 1 : Size.vector2Value.y);
            }
            Handles.DrawLine(_transform.position + new Vector3(Size.vector2Value.x, Size.vector2Value.y, 0), _transform.position + new Vector3(-Size.vector2Value.x, Size.vector2Value.y, 0));
            Handles.DrawLine(_transform.position + new Vector3(-Size.vector2Value.x, Size.vector2Value.y, 0), _transform.position + new Vector3(-Size.vector2Value.x, -Size.vector2Value.y, 0));
            Handles.DrawLine(_transform.position + new Vector3(-Size.vector2Value.x, -Size.vector2Value.y, 0), _transform.position + new Vector3(Size.vector2Value.x, -Size.vector2Value.y, 0));
            Handles.DrawLine(_transform.position + new Vector3(Size.vector2Value.x, -Size.vector2Value.y, 0), _transform.position + new Vector3(Size.vector2Value.x, Size.vector2Value.y, 0));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
