class NoMaterialsOnImport extends AssetPostprocessor {
   	
    function OnPreprocessModel() {
        var importer : ModelImporter = assetImporter as ModelImporter;        
        importer.importMaterials = false;
    }
}