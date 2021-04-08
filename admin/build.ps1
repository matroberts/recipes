# This script builds a self contained exe sitegen.exe, which is used to build the recipies site
# The exe gets copied into the folder above this one, which is the project root

dotnet publish  -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true .\sitegen\sitegen.csproj
copy .\sitegen\bin\Release\netcoreapp3.1\win-x64\publish\sitegen.exe ..\sitegen.exe