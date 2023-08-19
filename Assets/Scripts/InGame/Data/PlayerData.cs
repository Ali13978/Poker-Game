using Unity.Netcode;

// ReSharper disable InconsistentNaming

[System.Serializable]
public struct PlayerData : ISaveLoadData, INetworkSerializable
{
    public string NickName => _nickName;
    private string _nickName;

    public uint Stack => _stack;
    private uint _stack;

    public uint Money => _money;
    private uint _money;

    public PlayerData(string nickName, uint money, uint stack/* = 100*/)
    {
        _nickName = nickName;
        _stack = stack;
        _money = money;
    }

    public void SetDefaultValues()
    {
        _nickName = "Player";
        _stack = 100;
        _money = 30;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _nickName);
        serializer.SerializeValue(ref _stack);
        serializer.SerializeValue(ref _money);
    }
}