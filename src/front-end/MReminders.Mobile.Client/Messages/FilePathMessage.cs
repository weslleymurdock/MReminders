using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MReminders.Mobile.Client.Messages;

public class FilePathMessage(string path) : ValueChangedMessage<string>(path)
{
}
