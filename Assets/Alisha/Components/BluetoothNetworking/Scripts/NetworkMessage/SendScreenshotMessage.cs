using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendScreenshotMessage : MessageBase
{
    public short MessageType = (short)OperationMessageType.SendScreenshot;

    public int width;
    public int height;
    public byte[] ImageBytes;

    public override void Deserialize(NetworkReader reader)
    {
        MessageType = reader.ReadInt16();
        width = reader.ReadInt32();
        height = reader.ReadInt32();
        ImageBytes = reader.ReadBytesAndSize();
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(MessageType);
        writer.Write(width);
        writer.Write(height);
        writer.WriteBytesFull(ImageBytes);
    }
}
