using Avalonia.Media;

namespace HistoriaClinica.App.Icons;

/// <summary>
/// Geometrías de los iconos de Lucide usados en la interfaz.
/// Fuente: https://lucide.dev (licencia ISC).
/// </summary>
public static class LucideIcons
{
    public static Geometry Search { get; } = StreamGeometry.Parse("M21 21l-4.34-4.34M3 11A8 8 0 1 0 19 11A8 8 0 1 0 3 11Z");
    public static Geometry UserPlus { get; } = StreamGeometry.Parse("M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2M5 7A4 4 0 1 0 13 7A4 4 0 1 0 5 7ZM19 8L19 14M22 11L16 11");
    public static Geometry ClipboardPlus { get; } = StreamGeometry.Parse("M9 2H15A1 1 0 0 1 16 3V5A1 1 0 0 1 15 6H9A1 1 0 0 1 8 5V3A1 1 0 0 1 9 2ZM16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2M9 14h6M12 17v-6");
    public static Geometry FilePlus { get; } = StreamGeometry.Parse("M6 22a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h8a2.4 2.4 0 0 1 1.704.706l3.588 3.588A2.4 2.4 0 0 1 20 8v12a2 2 0 0 1-2 2zM14 2v5a1 1 0 0 0 1 1h5M9 15h6M12 18v-6");
    public static Geometry Copy { get; } = StreamGeometry.Parse("M10 8H20A2 2 0 0 1 22 10V20A2 2 0 0 1 20 22H10A2 2 0 0 1 8 20V10A2 2 0 0 1 10 8ZM4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2");
    public static Geometry Pencil { get; } = StreamGeometry.Parse("M21.174 6.812a1 1 0 0 0-3.986-3.987L3.842 16.174a2 2 0 0 0-.5.83l-1.321 4.352a.5.5 0 0 0 .623.622l4.353-1.32a2 2 0 0 0 .83-.497zM15 5l4 4");
    public static Geometry Trash2 { get; } = StreamGeometry.Parse("M10 11v6M14 11v6M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6M3 6h18M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2");
    public static Geometry X { get; } = StreamGeometry.Parse("M18 6 6 18M6 6l12 12");
    public static Geometry ChevronDown { get; } = StreamGeometry.Parse("M6 9l6 6 6-6");
    public static Geometry Settings { get; } = StreamGeometry.Parse("M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2zM12 15A3 3 0 1 0 12 9A3 3 0 1 0 12 15Z");
    public static Geometry Info { get; } = StreamGeometry.Parse("M2 12A10 10 0 1 0 22 12A10 10 0 1 0 2 12ZM12 16v-4M12 8h.01");
    public static Geometry FolderOpen { get; } = StreamGeometry.Parse("M6 14l1.5-2.9A2 2 0 0 1 9.24 10H20a2 2 0 0 1 1.94 2.5l-1.54 6a2 2 0 0 1-1.95 1.5H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h3.9a2 2 0 0 1 1.69.9l.81 1.2a2 2 0 0 0 1.67.9H18a2 2 0 0 1 2 2v2");
    public static Geometry DatabaseBackup { get; } = StreamGeometry.Parse("M3 5A9 3 0 1 0 21 5A9 3 0 1 0 3 5ZM3 12a9 3 0 0 0 5 2.69M21 9.3V5M3 5v14a9 3 0 0 0 6.47 2.88M12 12v4h4M13 20a5 5 0 0 0 9-3 4.5 4.5 0 0 0-4.5-4.5c-1.33 0-2.54.54-3.41 1.41L12 16");
    public static Geometry LogOut { get; } = StreamGeometry.Parse("M16 17l5-5-5-5M21 12H9M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4");
    public static Geometry User { get; } = StreamGeometry.Parse("M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2M8 7A4 4 0 1 0 16 7A4 4 0 1 0 8 7Z");
    public static Geometry History { get; } = StreamGeometry.Parse("M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8M3 3v5h5M12 7v5l4 2");
    public static Geometry Eye { get; } = StreamGeometry.Parse("M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0M9 12A3 3 0 1 0 15 12A3 3 0 1 0 9 12Z");
    public static Geometry EyeOff { get; } = StreamGeometry.Parse("M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49M14.084 14.158a3 3 0 0 1-4.242-4.242M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143M2 2l20 20");
    public static Geometry LogIn { get; } = StreamGeometry.Parse("M10 17l5-5-5-5M15 12H3M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4");
    public static Geometry Check { get; } = StreamGeometry.Parse("M20 6 9 17l-5-5");
    public static Geometry RotateCcw { get; } = StreamGeometry.Parse("M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8M3 3v5h5");
    public static Geometry UserSearch { get; } = StreamGeometry.Parse("M6 7A4 4 0 1 0 14 7A4 4 0 1 0 6 7ZM10.3 15H7a4 4 0 0 0-4 4v2M14 17A3 3 0 1 0 20 17A3 3 0 1 0 14 17ZM21 21l-1.9-1.9");
    public static Geometry Phone { get; } = StreamGeometry.Parse("M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z");
    public static Geometry FileText { get; } = StreamGeometry.Parse("M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Zm0 0 5 5M10 9H8M16 13H8M16 17H8");
    public static Geometry IdCard { get; } = StreamGeometry.Parse("M2 5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2Zm14 9h2m-2-4h2M6 14h4a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2H6a2 2 0 0 0-2 2v4a2 2 0 0 0 2 2Z");
}
