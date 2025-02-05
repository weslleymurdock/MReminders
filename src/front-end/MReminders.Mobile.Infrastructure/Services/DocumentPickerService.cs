#if IOS
using Foundation;
using FileProvider;
using UIKit;
using UniformTypeIdentifiers;

namespace MReminders.Mobile.Infrastructure.Services;

public class DocumentPickerService : NSObject, IUIDocumentPickerDelegate
{
    private TaskCompletionSource<(string FilePath, byte[] FileBytes, string ContentType)> _taskCompletionSource;

    public async Task<(string FilePath, byte[] FileBytes, string ContentType)> PickFileAsync()
    {
        _taskCompletionSource = new TaskCompletionSource<(string FilePath, byte[] FileBytes, string ContentType)>();

        var documentTypes = new string[] { UTTypes.Data.Identifier };
        var picker = new UIDocumentPickerViewController(documentTypes, UIDocumentPickerMode.Import)
        {
            Delegate = this
        };

        var vc = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .FirstOrDefault()?
            .Windows
            .FirstOrDefault(w => w.IsKeyWindow)?
            .RootViewController;

        vc?.PresentViewController(picker, true, () => { });

        return ("",[],"");
    }

    [Export("documentPicker:didPickDocumentsAtURLs:")]
    public async void DidPickDocumentsAtUrls(UIDocumentPickerViewController controller, NSUrl[] urls)
    {
        var url = urls.FirstOrDefault();
        if (url != null)
        {
            var securityEnabled = url.StartAccessingSecurityScopedResource();
            var filePath = url.Path;
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var contentType = GetMimeType(filePath);

            _taskCompletionSource.SetResult((filePath, fileBytes, contentType));
            if (securityEnabled)
            {
                url.StopAccessingSecurityScopedResource();
            }
        }
        else
        {
            _taskCompletionSource.SetResult((null, null, null));
        }
    }

    [Export("documentPickerWasCancelled:")]
    public void WasCancelled(UIDocumentPickerViewController controller)
    {
        _taskCompletionSource.SetResult((null, null, null));
    }

    private string GetMimeType(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath);
        if (!string.IsNullOrEmpty(fileExtension))
        {
            var mimeType = UTType.CreateFromExtension(fileExtension);
            return mimeType.ToString();
        }
        return "application/octet-stream";
    }
}

#endif