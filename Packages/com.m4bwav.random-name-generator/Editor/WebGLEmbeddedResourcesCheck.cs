using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RandomNameGeneratorLibrary.Editor
{
    // The name lists are embedded resources in RandomNameGeneratorLibrary.dll. Web builds leave embedded resources out
    // unless Player Settings > Publishing Settings > Use Embedded Resources is on, and every generator then throws
    // InvalidOperationException ("Embedded resource ... was not found") at runtime. Other platforms keep them.
    internal sealed class WebGLEmbeddedResourcesCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.WebGL && !PlayerSettings.WebGL.useEmbeddedResources)
            {
                Debug.LogError("Random Name Generator: turn on Player Settings > Publishing Settings > Use Embedded Resources "
                    + "(PlayerSettings.WebGL.useEmbeddedResources) for Web builds. Without it the name lists are left out "
                    + "and every generator throws InvalidOperationException at runtime.");
            }
        }
    }
}
