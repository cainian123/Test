
using AssetStream.Editor.AssetBundleSetting.ResourceModule;
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
                m_AssetInfoEditor = new AssetInfoEditor(this);
            }

            return m_AssetInfoEditor;
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
        //DrawSplitter(0);
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
        EditorGUI.DrawRect(splitterRect, Color.black);
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
                if (currentDragWidth < 200f)
                    currentDragWidth = 200f;
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
        
        /*var rect = EditorGUILayout.BeginVertical(GUILayout.Width(first_area_widht));
        //rect.y = 40;
        EditorGUILayout.BeginHorizontal(GUILayout.Height(40));
      
        // 设置标签样式
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.normal.textColor = Color.white;
            labelStyle.normal.background = Texture2D.grayTexture;
        }
        // 绘制标签
        EditorGUILayout.LabelField("模块名称", labelStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height));
        
        EditorGUILayout.BeginHorizontal();

        if (ResourceModuleDataManager.Instance.ResourceModuleConfigs != null)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
             style.alignment = TextAnchor.MiddleCenter;
             style.fontStyle = FontStyle.Normal;
             style.normal.textColor = Color.white;
            int i = 0;
            foreach (var key in ResourceModuleDataManager.Instance.ResourceModuleConfigs.Keys)
            { 
                i++;
               if (i % 2 == 0)
               {
                   style.normal.background = Texture2D.grayTexture;
               }
               else
               {
                   style.normal.background = Texture2D.blackTexture;
               }
               EditorGUILayout.LabelField(key,style);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();*/
        if(moduleGrouEditor.OnGUI(rect))
            Repaint();
    }

    private void DrawResourceModuleList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(second_area_width));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height));
        EditorGUILayout.BeginHorizontal();
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.clipping = TextClipping.Clip;
        GUILayout.Label("Second Area ---------------------------------------------------------------------->>>>>>>>>>>>>>>>>>>>>>>>>>>>>",style);

        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Second Area -----");
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawSearchArea()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(third_area_width));
        GUILayout.Label("Second Area");
        // 添加其他GUI元素
        EditorGUILayout.EndVertical();
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
    }
}
