using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using ManagedCommon;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.AskLLM;


public class Main : 
    IPlugin, IPluginI18n, IContextMenu,
    IReloadable, IDisposable {
    
    private PluginInitContext _context;
    private const string _iconPath = "Images/icon.png";
    public string Name { get; } = "Ask LLM";
    public string Description { get; } = "Ask LLM a question";
    
    public static string PluginID => "D1EAC3023B954468887ED504FD866EAC";
    
    private static Dictionary<string,string> _config = new Dictionary<string, string>();

    private static List<IntPtr> _focusWindow = new List<IntPtr>();
    
    public void Init(PluginInitContext context)
    {
        try
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            LoadConfig();
            Automation.AddAutomationFocusChangedEventHandler(FocusChangedHandler);
        }
        catch (Exception e)
        {
            File.WriteAllText(@"D:\error.txt", e.ToString());
            Log.Exception($"An error occurred while initializing the plugin: {e.Message}", e, typeof(Main));
        }
    }
    
    private static void FocusChangedHandler(object sender, AutomationFocusChangedEventArgs e)
    {
        var focusWindow = SelectedTextRetriever.GetForegroundWindow();
        if (_focusWindow.Count == 0)
        {
            _focusWindow.Add(focusWindow);
        }

        if (_focusWindow.Last() == focusWindow)
        {
            return;
        }
        
        _focusWindow.Add(focusWindow);
        
        if (_focusWindow.Count > 2)
        {
            _focusWindow.RemoveAt(0);
        }
    }
    

    public List<Result> Query(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);
        var results = new List<Result>();

        var result = new Result
        {
            Title = "Ask LLM",
            SubTitle = "Open LLM in default browser",
            QueryTextDisplay = query.Search,
            IcoPath = _iconPath,
        };
        
        result.Action = _ =>
        {
            try
            {
                string prompt = $@"{_config["prompt_without_selectedText"].Replace("{userInput}",query.Search)}";
        
                var selectedText = "";
                try
                {
                    selectedText = SelectedTextRetriever.GetSelectedText(_focusWindow.First());
                }
                catch (Exception exception)
                {
                    Logger.LogError($"Occur error when get selected text. Message: {exception.Message}", exception);
                }
        
                if (!string.IsNullOrEmpty(selectedText))
                {
                    prompt = _config["prompt"].Replace("{selectedText}",selectedText).Replace("{userInput}",query.Search);
                }
        
                string url = _config["url"].Replace("{prompt}",HttpUtility.UrlEncode(prompt));
                
                string tempFile = Path.Combine(Path.GetTempPath(), "open_url.html");
                string htmlContent = $"<html><head><meta http-equiv='refresh' content='0;url={url}' /></head><body></body></html>";
                File.WriteAllText(tempFile, htmlContent);

                Process.Start(new ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true
                });
                
                Thread.Sleep(1000);
                File.Delete(tempFile);
            }
            catch (Exception e)
            {
                Logger.LogError($"Occur error when open ChatLLM. Message: {e.Message}", e);
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return true;
        };
        
        results.Add(result);
        
        return results;
    }

    public string GetTranslatedPluginTitle() => Name;

    public string GetTranslatedPluginDescription() => Description;

    public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
    {
        return new List<ContextMenuResult>();
    }

    public void ReloadData()
    {
        if (_context == null)
        {
            return;
        }
        
        LoadConfig();
    }
    
    
    private void LoadConfig()
    {
        var dllPath = Assembly.GetExecutingAssembly().Location;
        var dllDirectory = Path.GetDirectoryName(dllPath);
        _config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText($"{dllDirectory}/appsettings.json"));
    }
    
    public void Dispose()
    {
        Automation.RemoveAutomationFocusChangedEventHandler(FocusChangedHandler);
    }

}