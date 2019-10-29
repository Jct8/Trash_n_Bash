using System;
using System.Collections;

public interface IDataSource
{
    string Id { get; set; }
    bool IsLoading { get; set; }
    bool IsLoaded { get; set; }
    string LoadError { get; set; }
    DateTime LoadedTime { get; set; }
    IEnumerator LoadAsync();
    void Load();
    void HandleOnLoaded();
}
