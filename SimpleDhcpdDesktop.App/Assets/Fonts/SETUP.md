# Font Awesome Setup Instructions

The application is now configured to use Font Awesome icons. To enable them, follow these steps:

## Step 1: Download Font Awesome

1. Visit https://fontawesome.com/download
2. Click "Download Free for Desktop"
3. You'll need to create a free account if you don't have one
4. Download the ZIP file

## Step 2: Extract the Font File

1. Extract the downloaded ZIP file
2. Navigate to the `webfonts` folder inside the extracted directory
3. Find the file `fa-solid-900.ttf` (Font Awesome 6 Free Solid)

## Step 3: Add Font to Project

1. Copy `fa-solid-900.ttf` to this directory:
   ```
   SimpleDhcpdDesktop.App/Assets/Fonts/fa-solid-900.ttf
   ```

2. The file should be automatically included as an AvaloniaResource (already configured in `.csproj`)

## Step 4: Enable Font in Styles

1. Open `SimpleDhcpdDesktop.App/Resources/Styles/Styles.axaml`
2. Find the commented Font Awesome FontFamily resource (around line 16)
3. Uncomment this line:
   ```xml
   <FontFamily x:Key="FontAwesome">avares://SimpleDhcpdDesktop.App/Assets/Fonts/fa-solid-900.ttf#Font Awesome 6 Free Solid</FontFamily>
   ```
4. Find the Font Awesome Icon Style section (around line 40)
5. Uncomment this line:
   ```xml
   <Setter Property="FontFamily" Value="{DynamicResource FontAwesome}" />
   ```

## Step 5: Rebuild

1. Rebuild the project: `dotnet build`
2. Run the application - Font Awesome icons should now display properly!

## Current Status

- ✅ Icon helper class created (`FontAwesomeIcons.cs`)
- ✅ All views updated to use Font Awesome Unicode characters
- ✅ Icon style created (ready for font file)
- ⏳ Font file needs to be added (see steps above)
- ⏳ Font resource needs to be uncommented in Styles.axaml

## Icons Currently Used

- **Save**: Floppy disk icon (`\uf0c7`)
- **Load/Open**: Folder open icon (`\uf07c`)
- **Add/Plus**: Plus icon (`\uf067`)
- **Remove/Close**: X mark icon (`\uf00d`)
- **New File**: File plus icon (`\uf319`)

## Alternative: Material Icons

If you prefer Material Icons instead of Font Awesome:
1. Download Material Icons from https://fonts.google.com/icons
2. Follow similar steps but use Material Icons font family name
3. Update the FontAwesomeIcons class with Material Icons Unicode values

## Troubleshooting

- **Icons show as boxes or question marks**: The font file isn't loaded. Check that:
  - The font file is in the correct location
  - The font resource is uncommented in Styles.axaml
  - The project has been rebuilt

- **Icons show as regular characters**: The font family isn't being applied. Verify:
  - The FontFamily resource key matches in Styles.axaml
  - The icon style is properly uncommented
