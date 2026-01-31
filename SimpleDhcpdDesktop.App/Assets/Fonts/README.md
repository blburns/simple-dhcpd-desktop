# Font Awesome Setup

To use Font Awesome icons in this application:

1. **Download Font Awesome 6 Free**:
   - Visit https://fontawesome.com/download
   - Download the "Free for Desktop" package
   - Extract the ZIP file

2. **Add Font Files**:
   - Copy `fa-solid-900.ttf` from the extracted `webfonts` folder
   - Place it in this directory (`Assets/Fonts/`)
   - The file should be at: `SimpleDhcpdDesktop.App/Assets/Fonts/fa-solid-900.ttf`

3. **Update Project File**:
   - The font file will be automatically included as an AvaloniaResource
   - If needed, verify it's included in `SimpleDhcpdDesktop.App.csproj`:
     ```xml
     <AvaloniaResource Include="Assets\**" />
     ```

4. **Enable Font in Styles**:
   - Open `Resources/Styles/Styles.axaml`
   - Uncomment the FontAwesome FontFamily resource
   - Uncomment the FontAwesome icon style

5. **Rebuild the Project**:
   - The Font Awesome icons will now be available throughout the application

## Alternative: Using Material Icons

If you prefer Material Icons instead:
1. Download Material Icons font from https://fonts.google.com/icons
2. Follow similar steps but use Material Icons font family name
