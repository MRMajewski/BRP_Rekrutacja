using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIViewWithSelectionStack
{
    public Stack<SelectionObjectInStack> GetSelectionStack();

    public void PushToSelectionStack(GameObject target, Action onCancel);

    public bool TryHandleCancel();

    public GameObject GetDefaultSelection();
}

public class SelectionObjectInStack
{
    public GameObject SelectedObject { get; private set; }
    public Action OnCancel { get; private set; }

    public SelectionObjectInStack(GameObject selectedObject, Action onCancel)
    {
        SelectedObject = selectedObject;
        OnCancel = onCancel;
    }
}
