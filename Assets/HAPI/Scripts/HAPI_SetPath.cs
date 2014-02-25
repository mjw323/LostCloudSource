using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif // UNITY_STANDALONE_WIN

namespace HAPI 
{
#if UNITY_EDITOR
	[ InitializeOnLoad ]
#endif // UNITY_EDITOR
	public class HAPI_SetPath {
#if UNITY_EDITOR
		static HAPI_SetPath()
		{
			setPath();
		}

		public static string getHoudiniPath()
		{
			string houdini_app_path = "";
#if UNITY_STANDALONE_WIN
			string hapi_path = System.Environment.GetEnvironmentVariable( "HAPI_PATH", 
																		  System.EnvironmentVariableTarget.Machine );
			if ( hapi_path == null || hapi_path.Length == 0 )
				hapi_path = System.Environment.GetEnvironmentVariable( "HAPI_PATH", 
																	   System.EnvironmentVariableTarget.User );
			if ( hapi_path == null || hapi_path.Length == 0 )
				hapi_path = System.Environment.GetEnvironmentVariable( "HAPI_PATH", 
																	   System.EnvironmentVariableTarget.Process );
			
			if ( hapi_path != null && hapi_path.Length > 0 )
			{
				Debug.Log( "Using Custom Houdini Path: " + hapi_path );
				houdini_app_path = hapi_path;
			}
			else
			{
				// HAPI_PATH variable not set therefore we must find another way to detect an installation of
				// Houdini. This step is platform dependant hence the #if's.
				
				string current_app_name = "Houdini Engine";
				while ( true )
				{
					try
					{
						houdini_app_path = getAppPath( current_app_name );
					}
					catch ( HAPI_Error )
					{
						if ( current_app_name == "Houdini" )
						{
							throw; // No correct installed app found.
						}
						else
						{
							current_app_name = "Houdini";
							continue;
						}
					}
					break;
				}
			}
#endif // UNITY_STANDALONE_WIN

			return houdini_app_path;
		}

		public static void setPath()
		{
			if ( prIsPathSet )
				return;
			
			try
			{
#if UNITY_STANDALONE_WIN
				string houdini_app_path = getHoudiniPath();
				string houdini_bin_path = houdini_app_path + "/bin";

				string path = System.Environment.GetEnvironmentVariable( "PATH", 
																		 System.EnvironmentVariableTarget.Machine );
			
				if ( !path.Contains( houdini_bin_path ) )
					if ( path != "" )
						path = houdini_bin_path + ";" + path;
					else
						path = houdini_bin_path;
			
				System.Environment.SetEnvironmentVariable( "PATH", path, System.EnvironmentVariableTarget.Process );
				Debug.Log( "DLL search path set to: " + path );
			
				prHoudiniPath = houdini_app_path;
				myIsPathSet = true;
#endif // UNITY_STANDALONE_WIN
			}
			catch ( HAPI_Error error )
			{
				Debug.LogError( error.ToString() );
			}
		}
		
		public static bool prIsPathSet { get { return myIsPathSet; } private set {} }
		public static string prHoudiniPath { get; private set; }

		private static string getAppPath( string app_name )
		{
			string app_path = "";
			
#if UNITY_4_3
			if ( BuildPipeline.isBuildingPlayer )
				return app_path;
#endif // UNITY_4_3

#if UNITY_STANDALONE_WIN
			// For Windows, we look at the registry entries made by the Houdini installer. We look for the 
			// "active version" key which gives us the most recently installed Houdini version. Using the
			// active version we find the registry made by that particular installer and find the install
			// path.

			RegistryKey local_machine = Registry.LocalMachine;

			RegistryKey sesi_key = local_machine.OpenSubKey( "Software\\Side Effects Software\\" + app_name );
			if ( sesi_key == null )
				throw new HAPI_Error( "No 32-bit " + app_name + " installation found!" );

			string correct_version = HAPI_Version.HOUDINI_MAJOR + "." + HAPI_Version.HOUDINI_MINOR + "." +
									 HAPI_Version.HOUDINI_BUILD;

			// Note the extra 0 for the "minor-minor" version that's needed here.
			string correct_version_key = HAPI_Version.HOUDINI_MAJOR + "." + HAPI_Version.HOUDINI_MINOR + 
										 ".0." + HAPI_Version.HOUDINI_BUILD;

			app_path = (string) sesi_key.GetValue( correct_version_key );
			if ( app_path == null || app_path.Length == 0 )
				throw new HAPI_Error( "The correct version (" + correct_version + ") of " + app_name + 
									  " was not found on the system!" );
			else if ( app_path.EndsWith( "\\" ) || app_path.EndsWith( "/" ) )
				app_path = app_path.Remove( app_path.Length - 1 );

			Debug.Log( "Linked-To " + app_name + " Install Path: " + app_path );
#else
			// TODO: Add support for other platforms (only whichever platforms the Unity Editor supports).

			//#error "Your current platform is not yet fully supported. Binaries search path not set."
			Debug.LogError( "Your current platform is not yet full support. Binaries search path not set." );

#endif // UNITY_STANDALONE_WIN

			return app_path;
		}

		private static bool myIsPathSet = false;
#endif // UNITY_EDITOR
	}
	
} // namespace HAPI
