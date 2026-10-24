using Avalonia;

namespace HistoriaClinica.App.Services;

public static class UiMetrics
{
    public static AppTextSize Tamano { get; private set; } = AppTextSize.Normal;

    public static double Scale => (double)(int)Tamano / 100d;

    public static event EventHandler? Changed;

    public static void Aplicar(AppTextSize tamano)
    {
        if (!Enum.IsDefined(tamano))
        {
            tamano = AppTextSize.Normal;
        }

        Tamano = tamano;

        if (Application.Current is { } app)
        {
            var escala = Scale;

            Set(app, "FontSizeH1", 26 * escala);
            Set(app, "FontSizeH2", 20 * escala);
            Set(app, "FontSizeSection", 16 * escala);
            Set(app, "FontSizeBody", 14 * escala);
            Set(app, "FontSizeSmall", 12 * escala);
            Set(app, "FontSizeCaption", 12 * escala);
            Set(app, "FontSizeListPreview", 13 * escala);
            Set(app, "FontSizeAvatar", 14 * escala);
            Set(app, "FontSizeEmptyTitle", 20 * escala);
            Set(app, "LineHeightBody", 20 * escala);

            Set(app, "ControlHeight", 38 * escala);
            Set(app, "ControlHeightSmall", 30 * escala);
            Set(app, "UserBadgeHeight", 48 * escala);
            Set(app, "DateFieldInnerHeight", 36 * escala);

            Set(app, "AvatarSize", 40 * escala);
            Set(app, "IconSize", 16 * escala);
            Set(app, "IconSizeSmall", 14 * escala);
            Set(app, "EmptyIconSize", 56 * escala);

            Set(app, "MultilineMinHeight", 100 * escala);
            Set(app, "FichaMotivoHeight", 76 * escala);
            Set(app, "FichaEnfermedadHeight", 92 * escala);
            Set(app, "FichaIndicacionesHeight", 100 * escala);

            Set(app, "UserNameMaxWidth", 180 * escala);
            Set(app, "EmptyDescriptionMaxWidth", 420 * escala);
            app.Resources["AvatarRadius"] = new CornerRadius(20 * escala);
        }

        Changed?.Invoke(null, EventArgs.Empty);
    }

    private static void Set(Application app, string key, double value) =>
        app.Resources[key] = value;
}
