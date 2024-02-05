
using AssetStream.Editor.AssetBundleSetting.ResourceModule;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI;
using UnityEditor;
using UnityEngine;

public class ResourceModuleBrowserMain : EditorWindow, IHasCustomMenu, ISerializationCallbackReceiver
{
    [MenuItem("Window/Resource Module", priority = 2100)]
    static void ShowWindow()
    {
        s_instance = null;
        instance.titleContent = new GUIContent("ResourceModule");
        instance.position = new Rect(Screen.width / 2f, Screen.height / 2f, first_area_widht + second_area_width+third_area_width, 800);
        instance.Show();
    }
    
    private static ResourceModuleBrowserMain s_instance = null;
    internal static ResourceModuleBrowserMain instance
    {
        get
        {
            if (s_instance == null)
                s_instance = GetWindow<ResourceModuleBrowserMain>();
            return s_instance;
        }
    }

    [SerializeField] public bool isAutoSave = true;
    [SerializeField]
    internal ResourceModuleGroupEditor m_GroupEditor;

    [SerializeField] internal AssetInfoEditor m_AssetInfoEditor;
    [SerializeField] internal SearchEditor m_SearchEditor;
    
    private ResourceModuleInfo currentSelectResourceModuleInfo;
    private ResourceModuleGroupEditor moduleGrouEditor
    {
        get
        {
            if (m_GroupEditor == null)
                m_GroupEditor = new ResourceModuleGroupEditor(this);
            return m_GroupEditor;
        }
    }

    private AssetInfoEditor assetInfoEditor
    {
        get
        {
            if (m_AssetInfoEditor == null)
            {
                m_AssetInfoEditor = new AssetInfoEditor(this,currentSelectResourceModuleInfo);
            }

            return m_AssetInfoEditor;
        }
    }
    
    private SearchEditor searchEditor
    {
        get
        {
            if (m_SearchEditor == null)
                m_SearchEditor = new SearchEditor(this);
            return m_SearchEditor;
        }
    }
    
    private const float top_barHeight = 30f;


    private static float first_area_widht = 400;
    private static float second_area_width = 600f;
    private static float third_area_width = 500f;


    private void OnEnable()
    {
        ResourceModuleDataManager.Instance.LoadData();
    }

    private void RegisterTab()
    {
        
    }
    
    private void OnGUI()
    {
        DrawTopBar();
        EditorGUILayout.BeginHorizontal();
        DrawResourceModulePackage();
        DrawResourceModuleList();
        DrawSplitter(1);
        DrawSearchArea();
        EditorGUILayout.EndHorizontal();
    }
    
    private bool isDraggingSplitter = false;
    private float mouseStartPositionX;
    private float startFirstWidth;
    private float currentDragWidth;
    private Rect splitterRect;
    private void DrawSplitter(int index)
    {
        splitterRect=GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(position.height),
            GUILayout.Width(1.0f));
        //EditorGUI.DrawRect(splitterRect, Color.black);
        if (index == 1)
        {
            Rect rect = new Rect(splitterRect.position,new Vector2(splitterRect.size.x + 2.0f,splitterRect.size.y));
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                isDraggingSplitter = true;
                mouseStartPositionX = Event.current.mousePosition.x;
                startFirstWidth = second_area_width;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDraggingSplitter = false;
            }

            if (isDraggingSplitter)
            {
                float delta = Event.current.mousePosition.x - mouseStartPositionX;
                currentDragWidth = Mathf.Max(0f, startFirstWidth + delta);
                if (currentDragWidth < 600f)
                    currentDragWidth = 600f;
                second_area_width = currentDragWidth;
                Repaint();
            }
        }
    }

    private void DrawTopBar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar,GUILayout.Height(top_barHeight));
        
        if (GUILayout.Button("Create ResourceModule", EditorStyles.toolbarButton))
        {
            CreateNewResourceModule();
        }

        isAutoSave = GUILayout.Toggle(isAutoSave, "Auto Save");

        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
        {
            Debug.Log("save");
        }
        
        GUILayout.FlexibleSpace();
        
        GUILayout.EndHorizontal();
    }

    private GUIStyle labelStyle;
    private Vector2 scrollPosition;
    private void DrawResourceModulePackage()
    {
        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(first_area_widht+2),GUILayout.Height(position.height - top_barHeight));
        
        if(moduleGrouEditor.OnGUI(rect))
            Repaint();
    }

    private void DrawResourceModuleList()
    {
        
        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(second_area_width+2),GUILayout.Height(position.height - top_barHeight));
        if(assetInfoEditor.OnGUI(rect))
            Repaint();
    }

    private void DrawSearchArea()
    {
        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(position.width),GUILayout.Height(position.height - top_barHeight));
        if(searchEditor.OnGUI(rect))
            Repaint();
    }
    
    private void CreateNewResourceModule()
    {
        if (!ResourceModuleDataManager.Instance.CreateResourceModule())
        {
           EditorUtility.DisplayDialog("Error", "Create ResourceModule fail", "OK");
           return;
        }
        if(moduleGrouEditor != null)
            moduleGrouEditor.Reload();
    }

    public void ShowAssetList(ResourceModuleInfo info)
    {
        if(currentSelectResourceModuleInfo != null && currentSelectResourceModuleInfo.packageName.Equals(info.packageName))
            return;
        currentSelectResourceModuleInfo = info;
        if (assetInfoEditor != null)
        {
            assetInfoEditor.Reload(info);
        }
    }
    
    public void AddItemsToMenu(GenericMenu menu)
    {
        
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
       
    }
    
    void OnDisable()
    {
        if(m_GroupEditor != null)
            m_GroupEditor.OnDisable();
        if(m_AssetInfoEditor != null)
            m_AssetInfoEditor.OnDisable();
        m_GroupEditor = null;
        m_AssetInfoEditor = null;
        currentSelectResourceModuleInfo = null;
    }
}
