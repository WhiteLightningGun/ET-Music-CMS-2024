namespace UpLoader_For_ET.StaticClasses;

using System.Linq;

/// <summary>
/// Ensures the uploaded file hasn't been maliciously renamed and 
/// has a byte header corresponding to the permitted file formats.
/// Will return a bool upon completion.
/// </summary>
public static class UploadCheck
{
    private static List<byte[]> audioFileHeaders = new()
    {
        new byte[] { 0x49, 0x44, 0x33, }, // mp3WithID3v2Bytes
        new byte[] { 0x4F, 0x67, 0x67, 0x53 } // oggVorbisHeader
    };
    // using IEnumerable<byte> could be better

    /// <summary>
    /// Takes a byte array of arbitrary length and compares with the file signatures of the approved file formats: OGG and MP3. 
    /// Returns a bool if successful.
    /// </summary>
    /// <param name="loadedArray"></param>
    /// <returns></returns>
    public static bool CheckFormat(byte[] loadedArray)
    {
        
        if(loadedArray.Length < 8) // an unusually short byteArray will be rejected, preventing a program error
        {
            return false;
        }

        foreach(byte[] headerArray in audioFileHeaders)
        {
            if(ByteHeaderCompare(loadedArray, headerArray))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ByteHeaderCompare(byte[] byteArray, byte[] headerArray)
    //headerArray MUST be at equal or greater length than byteArray, it is hard to imagine a situation where this will not be the case
    {
        for (int i = 0; i < headerArray.Count(); i++)
        {
            if(byteArray[i] != headerArray[i])
            {
                return false;
            }
        }
        return true;
    }

}