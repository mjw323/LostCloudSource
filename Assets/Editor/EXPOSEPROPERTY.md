How to Correctly Expose Properties
==================================
## Exposing the Property
You can expose properties with `ExposePropertyAttribute`. An example:

    [ExposeProperty]
    public Transform ExposedTransform {
        get { return exposedTransform; }
        set { exposedTransform = value; }
    }
    
    [HideInInspector] [SerializeField] private Transform exposedTransform;

The property must be read/write (have both a getter and a setter), and the backing field for the property must have both `HideInInspectorAttribute` and `SerializeFieldAttribute` applied to it.

The object will also need a custom editor in the Editor directory:

    [CustomEditor(typeof(Foo))]
    public FooEditor : Editor {
        public void OnEnable() {
            instance = (Foo)target;
            properties = Property.GetProperties(instance);
        }
        
        public override void OnInspectorGUI() {
            if (instance == null) {
                return;
            }
            
            DrawDefaultInspector();
            Property.ExposeProperties(properties);
            if (GUI.changed) {
                EditorUtility.SetDirty(instance);
            }
        }
    }

## Making It Safe
The property's setter must safely work when used by the Editor. Here's an example which won't work:

    [ExposeProperty]
    public Transform ExposedTransform {
        get { return exposedTransform; }
        set {
            exposedTransform = value;
            ComputeDistance();
        }
    }
    
    private void Awake() {
        transform = GetComponent<Transform>();
    }
    
    private void ComputeDistance() {
        distance = (exposedTransform - transform).magnitude;
    }
    
    private void Start() {
        if (exposedTransform != null) {
            ComputeDistance();
        }
    }
    
    private void Update() {
        if (exposedTransform != null) {
            ...
        }
    }
    
    [HideInInspector] [SerializeField] private Transform exposedTransform;
    new private Transform transform;
    private float distance;
    
Because `transform` is a null reference until after `Awake()` has finished, attempting to use the property's setter will result in a `NullReferenceException`.

Here's one way that the setter can be made safe:

    [ExposeProperty]
    public Transform ExposedTransform {
        get { return exposedTransform; }
        set {
            exposedTransform = value;
    #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
    #endif
                ComputeDistance();
        }
    }
    
    private void Awake() {
        transform = GetComponent<Transform>();
    }
    
    private void ComputeDistance() {
        distance = (exposedTransform - transform).magnitude;
    }
    
    private void Start() {
        if (exposedTransform != null) {
            ComputeDistance();
        }
    }
    
    private void Update() {
        if (exposedTransform != null) {
            ...
        }
    }
    
    [HideInInspector] [SerializeField] private Transform exposedTransform;
    new private Transform transform;
    private float distance;
    
Now the setter won't call `ComputeDistance` inside the Editor unless it's in Play Mode, in which case the object will have awoken and `transform` will not be a null reference.

In general, this kind of fix can be performed as long as your setter's side effect(s) can be deferred to inside `Awake()` or `Start()`. If your setter can't assign to its backing field without accessing a null reference when the Editor isn't in Play Mode, you must either refactor it until it can or forgo exposing the property to the Inspector.

### Quick Note
Be careful when disabling behavior using preprocessor directives. If your setter has multiple side effects, you'll need a second `#if` clause to handle the closing bracket:

    #if UNITY_EDITOR
        if (EditorApplication.isPlaying) {
    #endif
            SideEffectOne();
            SideEffectTwo();
    #if UNITY_EDITOR
        }
    #endif