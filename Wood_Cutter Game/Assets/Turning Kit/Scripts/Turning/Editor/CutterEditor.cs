using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Cutter))]
public class CutterEditor : Editor
{
    private ReorderableList list;
    SerializedProperty chiselPrefab;
    SerializedProperty MaterialChisel;
    SerializedProperty Size;

    List<Vector2> BottomCutter = new List<Vector2>();

    private Transform _transform;


    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("contour"), false, true, true, true);
        chiselPrefab = serializedObject.FindProperty("chiselPrefab");
        MaterialChisel = serializedObject.FindProperty("MaterialChisel");
        Size = serializedObject.FindProperty("Size");

        // Draw header label
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Contour");
        };

        // Draw each element in the list
        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, 40, EditorGUIUtility.singleLineHeight), (index + 1).ToString());

                EditorGUI.PropertyField(
                     new Rect(rect.x + 40, rect.y, rect.width - 40, EditorGUIUtility.singleLineHeight),
                     element, GUIContent.none);
            };

        _transform = (target as Cutter).transform;
    }

    public override void OnInspectorGUI()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
            GUI.enabled = true;

            serializedObject.Update();

            EditorGUILayout.PropertyField(chiselPrefab);
            EditorGUILayout.PropertyField(MaterialChisel);
            EditorGUILayout.PropertyField(Size);
            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
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
            var fmh_82_21_638594974455013327 = Quaternion.identity; Size.vector2Value = (Handles.FreeMoveHandle(
                    _transform.position + (Vector3)Size.vector2Value,
                    HandleUtility.GetHandleSize(_transform.position + (Vector3)Size.vector2Value) / 5,
                    Vector3.one * 0.05f,
                    Handles.CubeHandleCap)
                - _transform.position);
            if (Size.vector2Value.x < 0.1f || Size.vector2Value.y < 0.1f)
                Size.vector2Value = new Vector2(Size.vector2Value.x < 0.1f ? 0.1f : Size.vector2Value.x, Size.vector2Value.y < 0.1f ? 0.1f : Size.vector2Value.y);

            BottomCutter.Clear();
            BottomCutter.AddRange((target as Cutter).GetBottomCutter());

            Handles.DrawLine(_transform.position + (Vector3)BottomCutter[0], _transform.position + (Vector3)BottomCutter[1]);
            Handles.DrawLine(_transform.position + (Vector3)BottomCutter[1], _transform.position + (Vector3)BottomCutter[2]);
            Handles.DrawLine(_transform.position + (Vector3)BottomCutter[2], _transform.position + (Vector3)BottomCutter[3]);

            if (list.count == 0)
                Handles.DrawLine(_transform.position + (Vector3)BottomCutter[3], _transform.position + (Vector3)BottomCutter[0]);
            else
            {
                for (int i = 0; i < list.count; i++)
                {
                    SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(i);

                    Handles.color = Color.green;
                    if (i == 0)
                        Handles.DrawLine(_transform.position + (Vector3)BottomCutter[3], _transform.position + (Vector3)element.vector2Value);
                    else
                    {
                        SerializedProperty element_prev = list.serializedProperty.GetArrayElementAtIndex(i - 1);
                        Handles.DrawLine(_transform.position + (Vector3)element_prev.vector2Value, _transform.position + (Vector3)element.vector2Value);
                    }
                }
                Handles.DrawLine(_transform.position + (Vector3)list.serializedProperty.GetArrayElementAtIndex(list.count - 1).vector2Value, _transform.position + (Vector3)BottomCutter[0]);

                for (int i = 0; i < list.count; i++)
                {
                    SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(i);

                    if (i == list.index)
                    {
                        Handles.color = Color.green;
                        var fmh_125_33_638594974455028373 = Quaternion.identity; element.vector2Value = Handles.FreeMoveHandle(
                                _transform.position + (Vector3)element.vector2Value,
                                HandleUtility.GetHandleSize(_transform.position + (Vector3)element.vector2Value) / 5,
                                Vector3.one * 0.05f,
                                Handles.CubeHandleCap)
                            - _transform.position;

                        float x_min = (i == 0) ? BottomCutter[3].x : list.serializedProperty.GetArrayElementAtIndex(i - 1).vector2Value.x;
                        float x_max = (i == list.count - 1) ? BottomCutter[0].x : list.serializedProperty.GetArrayElementAtIndex(i + 1).vector2Value.x;

                        element.vector2Value = new Vector2(Mathf.Clamp(element.vector2Value.x, x_min, x_max), element.vector2Value.y < 0 ? 0 : element.vector2Value.y);
                    }
                    else
                    {
                        Handles.color = Color.red;
                        if (Handles.Button(_transform.position + (Vector3)element.vector2Value, Quaternion.identity, HandleUtility.GetHandleSize(_transform.position + (Vector3)element.vector2Value) / 8, 0.05f, Handles.SphereHandleCap))
                            list.index = i;
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
