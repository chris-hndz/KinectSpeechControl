using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;


public class FacetrackingWrapper 
{
	public enum NuiImageResolution
	{
		resolutionInvalid = -1,
		resolution80x60 = 0,
		resolution320x240 = 1,
		resolution640x480 = 2,
		resolution1280x960 = 3     // for hires color only
	}
	
	public static class Constants
	{
		public const int ColorImageWidth = 640;
		public const int ColorImageHeight = 480;
		public const NuiImageResolution ColorImageResolution = NuiImageResolution.resolution640x480;
		
		public const int DepthImageWidth = 640;
		public const int DepthImageHeight = 480;
		public const NuiImageResolution DepthImageResolution = NuiImageResolution.resolution640x480;
		
		public const bool IsNearMode = true;
	}
	
	[Flags]
    public enum NuiInitializeFlags : uint
    {
		UsesAudio = 0x10000000,
        UsesDepthAndPlayerIndex = 0x00000001,
        UsesColor = 0x00000002,
        UsesSkeleton = 0x00000008,
        UsesDepth = 0x00000020,
		UsesHighQualityColor = 0x00000040
    }

	public struct FaceRect
	{
	    public int x;
	    public int y;
	    public int width;
	    public int height;
	}
	

#if USE_KINECT_MANAGER
	public static int InitKinectSensor(int iColorResolution, int iDepthResolution, bool bNearMode)
	{
		return 0;
	}
	
	public static void ShutdownKinectSensor()
	{
	}
#else
	[DllImport(@"KinectUnityWrapper", EntryPoint = "InitKinectSensor")]
	public static extern int InitKinectSensor(NuiInitializeFlags dwFlags, bool bEnableEvents, int iColorResolution, int iDepthResolution, bool bNearMode);

	[DllImport(@"KinectUnityWrapper", EntryPoint = "ShutdownKinectSensor")]
    public static extern void ShutdownKinectSensor();
	
	public static int InitKinectSensor(int iColorResolution, int iDepthResolution, bool bNearMode)
	{
		int hr = InitKinectSensor(NuiInitializeFlags.UsesColor|NuiInitializeFlags.UsesDepthAndPlayerIndex|NuiInitializeFlags.UsesSkeleton, true, iColorResolution, iDepthResolution, bNearMode);
		return hr;
	}
#endif
	
	// DLL Imports to pull in the necessary Unity functions to make the Kinect go.
	[DllImport("KinectUnityWrapper")]
	public static extern int InitFaceTracking();
	[DllImport("KinectUnityWrapper")]
	public static extern void FinishFaceTracking();
	[DllImport("KinectUnityWrapper")]
	public static extern int UpdateFaceTracking();
	
	[DllImport("KinectUnityWrapper")]
	public static extern bool IsFaceTracked();
	[DllImport("KinectUnityWrapper")]
	public static extern uint GetFaceTrackingID();
	[DllImport("KinectUnityWrapper")]
	public static extern int GetAnimUnitsCount();
	[DllImport("KinectUnityWrapper")]
	public static extern bool GetAnimUnits([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.R4)] float[] afAU, ref int iAUCount);
	[DllImport("KinectUnityWrapper")]
	public static extern bool IsShapeConverged();
	[DllImport("KinectUnityWrapper")]
	public static extern int GetShapeUnitsCount();
	[DllImport("KinectUnityWrapper")]
	public static extern bool GetShapeUnits([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.R4)] float[] afSU, ref int iSUCount);
	[DllImport("KinectUnityWrapper")]
	public static extern int GetShapePointsCount();
	[DllImport("KinectUnityWrapper")]
	public static extern bool GetShapePoints([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.R4)] float[] avPoints, ref int iPointsCount);

	///// added by Takefumi
	[DllImport("KinectUnityWrapper")]
	public static extern int Get3DShapePointsCount();
	[DllImport("KinectUnityWrapper")]
	public static extern bool Get3DShapePoints([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.R4)] float[] av3DPoints, ref int i3DPointsCount);
	///// end of added

	[DllImport(@"KinectUnityWrapper.dll")]
	//public static extern bool GetColorFrameData([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.U1)] byte[] btVideoBuf, ref uint iVideoBufLen, bool bGetNewFrame);
	//public static extern bool GetColorFrameData([MarshalAs(UnmanagedType.LPArray, SizeConst = 640 * 480 * 4, ArraySubType = UnmanagedType.U1)] byte[] btVideoBuf, ref uint iVideoBufLen, bool bGetNewFrame);
	public static extern bool GetColorFrameData(IntPtr btVideoBuf, ref uint iVideoBufLen, bool bGetNewFrame);

	[DllImport(@"KinectUnityWrapper.dll")]
	public static extern bool GetHeadPosition(ref Vector4 pvHeadPos);
	[DllImport(@"KinectUnityWrapper.dll")]
	public static extern bool GetHeadRotation(ref Vector4 pvHeadRot);
	[DllImport(@"KinectUnityWrapper.dll")]
	public static extern bool GetHeadScale(ref Vector4 pvHeadScale);
	
	[DllImport(@"KinectUnityWrapper.dll")]
	public static extern bool GetFaceRect(ref FaceRect pRectFace);

	
	public static int GetImageWidth()
	{
		return Constants.ColorImageWidth;
	}
	
	public static int GetImageHeight()
	{
		return Constants.ColorImageHeight;
	}
	
	public static bool PollVideo(ref byte[] videoBuffer, ref Color32[] colorImage)
	{
		uint videoBufLen = (uint)videoBuffer.Length;

		var pVideoData = GCHandle.Alloc(videoBuffer, GCHandleType.Pinned);
		bool newColor = GetColorFrameData(pVideoData.AddrOfPinnedObject(), ref videoBufLen, true);
		pVideoData.Free();

		if (newColor)
		{
			int totalPixels = colorImage.Length;
			
			for (int pix = 0; pix < totalPixels; pix++)
			{
				int ind = totalPixels - pix - 1;
				int src = pix << 2;
				
				colorImage[ind].r = videoBuffer[src + 2]; // pixels[pix].r;
				colorImage[ind].g = videoBuffer[src + 1]; // pixels[pix].g;
				colorImage[ind].b = videoBuffer[src]; // pixels[pix].b;
				colorImage[ind].a = 255;
			}
		}
		
		return newColor;
	}
	
	
	// Copy a resource file to the target
	private static bool CopyResourceFile(string targetFilePath, string resFileName, ref bool bOneCopied, ref bool bAllCopied)
	{
		TextAsset textRes = Resources.Load(resFileName, typeof(TextAsset)) as TextAsset;
		if(textRes == null)
		{
			bOneCopied = false;
			bAllCopied = false;
			
			return false;
		}
		
		FileInfo targetFile = new FileInfo(targetFilePath);
		if(!targetFile.Directory.Exists)
		{
			targetFile.Directory.Create();
		}
		
		if(!targetFile.Exists || targetFile.Length !=  textRes.bytes.Length)
		{
			if(textRes != null)
			{
				using (FileStream fileStream = new FileStream (targetFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					fileStream.Write(textRes.bytes, 0, textRes.bytes.Length);
				}
				
				bool bFileCopied = File.Exists(targetFilePath);
				
				bOneCopied = bOneCopied || bFileCopied;
				bAllCopied = bAllCopied && bFileCopied;
				
				return bFileCopied;
			}
		}
		
		return false;
	}
	
	// Copies the needed resources into the project directory
	public static bool EnsureKinectWrapperPresence()
	{
		bool bOneCopied = false, bAllCopied = true;
		
		CopyResourceFile("KinectUnityWrapper.dll", "KinectUnityWrapper.dll", ref bOneCopied, ref bAllCopied);
		CopyResourceFile("KinectInteraction180_32.dll", "KinectInteraction180_32.dll", ref bOneCopied, ref bAllCopied);
		CopyResourceFile("FaceTrackData.dll", "FaceTrackData.dll", ref bOneCopied, ref bAllCopied);
		CopyResourceFile("FaceTrackLib.dll", "FaceTrackLib.dll", ref bOneCopied, ref bAllCopied);
		CopyResourceFile("KinectBackgroundRemoval180_32.dll", "KinectBackgroundRemoval180_32.dll", ref bOneCopied, ref bAllCopied);
		
		CopyResourceFile("msvcp100d.dll", "msvcp100d.dll", ref bOneCopied, ref bAllCopied);
		CopyResourceFile("msvcr100d.dll", "msvcr100d.dll", ref bOneCopied, ref bAllCopied);
		
		return bOneCopied && bAllCopied;
	}
	
}
