using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperParallax : MonoBehaviour
{

    [SerializeField]
    protected GameObject fastestDayLayer;
    [SerializeField]
    protected GameObject mediumDayLayer;
    [SerializeField]
    protected GameObject slowDayLayer;
    [SerializeField]
    protected GameObject fastestNightLayer;
    [SerializeField]
    protected GameObject mediumNightLayer;
    [SerializeField]
    protected GameObject slowNightLayer;

    [SerializeField]
    protected float movementIncrement = 0.5f;

    protected JumperManagerGame gm;

    protected Transform followTarget;
    protected Vector2 fastLastPos;
    protected Vector2 mediumLastPos;
    protected Vector2 slowLastPos;
    protected List<RectTransform> layers = new List<RectTransform>();
    protected List<Vector2> lastPositions = new List<Vector2>();
    protected Vector2 previousPosition;

    private void Start()
    {
        gm = JumperManagerGame.Instance;
        if (gm.Night)
        {
            layers = new List<RectTransform>
            {
                slowNightLayer.GetComponent<RectTransform>(), 
                mediumNightLayer.GetComponent<RectTransform>(), 
                fastestNightLayer.GetComponent<RectTransform>()
            };
        }
        else
        {
            layers = new List<RectTransform>
            {
                slowDayLayer.GetComponent<RectTransform>(), 
                mediumDayLayer.GetComponent<RectTransform>(), 
                fastestDayLayer.GetComponent<RectTransform>()
            };
        }
        followTarget = gm.MainCam.transform;
        fastLastPos = mediumLastPos = slowLastPos = new Vector2(followTarget.position.x, followTarget.position.y);
        lastPositions.Add(slowLastPos);
        lastPositions.Add(mediumLastPos);
        lastPositions.Add(fastLastPos);
    }

    void FixedUpdate()
    {
        if (gm.MainCam == null) { return; }
        //get negative of cameras movement since last frame
        //apply a portion of it to the movement of each slow/medium/fast canvas
        //do nothing if camera hasnt moved
        var diff = (Vector2)followTarget.position - previousPosition;
        previousPosition = followTarget.position;
        int i = 1;
        foreach( var layer in layers)
        {
            layer.anchoredPosition = new Vector2(layer.anchoredPosition.x - (i * diff.x) * movementIncrement, 
                                                 layer.anchoredPosition.y - (i * diff.y) * movementIncrement);
            i++;
        }
    }
}
