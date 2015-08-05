using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;

namespace Prism.Debugging
{
    public static class DisgustingHacks
    {
#if DEV_BUILD
        /// <summary>
        /// Very ghetto-ly loads an embedded asset through the provided <see cref="ContentManager"/>
        /// by saving it to a temp file, loading it with the content manager, then deleting the temp file.
        /// </summary>
        /// <typeparam name="T">Type of content. Same as Content.Load&lt;T&gt;"/></typeparam>
        /// <param name="resourceName">Name of the manifest resource.</param>
        /// <param name="content">The <see cref="ContentManager"/> to load the content through.</param>
        /// <param name="fakeSaveFileName">The file name for the fake file to save. Make sure it doesn't overrite any other file because it gets deleted afterward.</param>
        /// <param name="fakeContentPath">The content path of the fake file saved, relative to the content directory.</param>
        /// <returns></returns>
        public static T LoadResourceThroughContentManager<T>(string resourceName)
        {
            var splitFileName = resourceName.Split('.');
            var fakeSaveFileName = String.Empty;
            var fakeContentPath = new StringBuilder();

            for (int i = 0; i < splitFileName.Length - 1; i++)
                fakeContentPath.Append((i != 0 ? "." : String.Empty) + splitFileName[i]);

            fakeSaveFileName = Path.Combine(Main.instance.Content.RootDirectory, fakeContentPath.ToString() + "." + splitFileName[splitFileName.Length - 1]);

            if (File.Exists(fakeSaveFileName))
                File.Delete(fakeSaveFileName);

            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var fileStream = File.OpenWrite(fakeSaveFileName))
            {
                resourceStream.CopyTo(fileStream);
            }

            T result = Main.instance.Content.Load<T>(fakeContentPath.ToString());

            File.Delete(fakeSaveFileName);

            return result;

        }
#endif
    }
}
