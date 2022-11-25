using System;
using System.Net.WebSockets;
using UnityEngine;

[Serializable]
public class HoldButton: AbstractButton
{
    [SerializeField] protected bool released;
    public Enums.TypeHold type;

    [SerializeField] protected float max;

    [SerializeField] protected float min;
    private InputValue inputValue;

    public override void Init(GameObject obj = null)
    {
        objectRef = obj;

        if (obj != null) obj.TryGetComponent<UIButton>(out manager);
    }

    public InputValue getInputValue() => inputValue;
    public bool getRelease() => released;
    public void setInputValue(InputValue input) => inputValue = input;
    public void setMinMax(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    public void setRelease(bool value) { released = value; }
    
    
    /// <summary>
    /// Function depending on the type:
    /// <br>-> Moveable: Move the linkedObjs. </br>
    /// <br>-> Increase/Decrease: Needs a InputValue to increment, being the increment value an Abs.</br>
    /// <br>-> Alternate: Needs a InputValue to increment till a max/min then changing the sign of the increment value</br>
    /// </summary>
    public override void Function()
    {
        switch (type)
        {
            case Enums.TypeHold.MOVEABLE:
                Move();
                return;
            case Enums.TypeHold.DECREASE:
            case Enums.TypeHold.INCREASE:
                inputValue.floatValue = Increment();
                return;
            case Enums.TypeHold.ALTERNATE:
                inputValue.floatValue = Alternate(max, min);
                return;
        }
    }


    private float Increment()
    {
        //In this case both are zero
        if (min == max) return inputValue.floatValue;


        if (!released) return (inputValue.floatValue + inputValue.incrementValue);
        return inputValue.floatValue;
    }
    /// <summary>
    /// Set Min and Max before you call the function 
    /// <br>function: setMinMax(min, max)</br>
    /// </summary>
    private float Alternate(float max, float min = default)
    {
        if (!released)
        {
            var value = inputValue.floatValue;
            var increment = inputValue.incrementValue;
            if (value >= max) { value = max; increment = -increment; }
            else if (value <= min) { value = min; increment = -increment; }
            else value += increment;

            inputValue.incrementValue = increment;
            return value;
        }
        return inputValue.floatValue;
    }


    private void Move()
    {
        foreach (GameObject obj in manager.linkedObjects)
        {
            if (obj.GetComponent<Animator>().enabled) obj.GetComponent<Animator>().enabled = false;
            var newPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            newPosition = MoveBoundaries(newPosition);
            var offset = newPosition - Camera.main.ScreenToViewportPoint(objectRef.transform.position);

            obj.transform.position += new Vector3(Screen.height * offset.x, Screen.width * offset.y);

        }
    }
    private Vector3 MoveBoundaries(Vector3 newPosition)
    {
        if (newPosition.x <= 0.05f) newPosition.x = 0.05f;
        if (newPosition.x >= 0.95f) newPosition.x = 0.95f;
        if (newPosition.y <= 0.05f) newPosition.y = 0.05f;
        if (newPosition.y >= 0.95f) newPosition.y = 0.95f;
        return newPosition;
    }
}