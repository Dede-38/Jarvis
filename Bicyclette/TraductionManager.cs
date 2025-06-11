using System.Collections.Generic;

namespace Bicyclette
{
    public static class TraductionManager
    {
        public static string Langue { get; set; } = Properties.Settings.Default.Langue;

        private static readonly Dictionary<string, Dictionary<string, string>> Textes = new()
        {
            ["ModèleIA"] = new()
            {
                ["fr"] = "Modèle IA",
                ["en"] = "AI Model",
                ["es"] = "Modelo IA",
                ["it"] = "Modello IA",
                ["ru"] = "Модель ИИ",
                ["zh"] = "人工智能模型",
                ["ja"] = "AIモデル"
            },
            ["Nouvelle"] = new()
            {
                ["fr"] = "➕ Nouvelle",
                ["en"] = "➕ New",
                ["es"] = "➕ Nueva",
                ["it"] = "➕ Nuova",
                ["ru"] = "➕ Новая",
                ["zh"] = "➕ 新建",
                ["ja"] = "➕ 新規"
            },
            ["Supprimer"] = new()
            {
                ["fr"] = "Supprimer",
                ["en"] = "Delete",
                ["es"] = "Eliminar",
                ["it"] = "Elimina",
                ["ru"] = "Удалить",
                ["zh"] = "删除",
                ["ja"] = "削除"
            },
            ["ErreurIA"] = new()
            {
                ["fr"] = "❌ Erreur lors de la connexion à l'IA",
                ["en"] = "❌ Error connecting to AI",
                ["es"] = "❌ Error al conectar con la IA",
                ["it"] = "❌ Errore di connessione all'IA",
                ["ru"] = "❌ Ошибка подключения к ИИ",
                ["zh"] = "❌ 连接人工智能出错",
                ["ja"] = "❌ AIへの接続エラー"
            },
            ["Typing"] = new()
            {
                ["fr"] = "🤖 : Écriture...",
                ["en"] = "🤖: Typing...",
                ["es"] = "🤖: Escribiendo...",
                ["it"] = "🤖: Scrivendo...",
                ["ru"] = "🤖: Печатает...",
                ["zh"] = "🤖: 输入中...",
                ["ja"] = "🤖: 入力中..."
            },
            ["MicroInactif"] = new()
            {
                ["fr"] = "Micro non disponible",
                ["en"] = "Micro unavailable",
                ["es"] = "Micrófono no disponible",
                ["it"] = "Microfono non disponibile",
                ["ru"] = "Микрофон недоступен",
                ["zh"] = "麦克风不可用",
                ["ja"] = "マイクが利用できません"
            },
            ["MicroVeille"] = new()
            {
                ["fr"] = "Activer l'écoute",
                ["en"] = "Enable listening",
                ["es"] = "Activar escucha",
                ["it"] = "Attiva ascolto",
                ["ru"] = "Включить прослушивание",
                ["zh"] = "启用监听",
                ["ja"] = "リスニングを有効にする"
            },
            ["MicroEcoute"] = new()
            {
                ["fr"] = "Écoute activée (dites 'Jarvis')",
                ["en"] = "Listening... (say 'Jarvis')",
                ["es"] = "Escuchando... (di 'Jarvis')",
                ["it"] = "In ascolto... (dici 'Jarvis')",
                ["ru"] = "Слушает... (скажите 'Jarvis')",
                ["zh"] = "正在监听...（说“Jarvis”）",
                ["ja"] = "リスニング中...（「ジャーヴィス」と言ってください）"
            },

            // Nouveaux textes ajoutés
            ["MicroBoutonTooltip"] = new()
            {
                ["fr"] = "Activer ou désactiver le microphone",
                ["en"] = "Toggle microphone on/off",
                ["es"] = "Activar o desactivar micrófono",
                ["it"] = "Attiva o disattiva il microfono",
                ["ru"] = "Включить или выключить микрофон",
                ["zh"] = "开启或关闭麦克风",
                ["ja"] = "マイクのオン/オフを切り替え"
            },
            ["ImageBoutonTooltip"] = new()
            {
                ["fr"] = "Ajouter une image à la conversation",
                ["en"] = "Add an image to the conversation",
                ["es"] = "Agregar una imagen a la conversación",
                ["it"] = "Aggiungi un'immagine alla conversazione",
                ["ru"] = "Добавить изображение в разговор",
                ["zh"] = "向对话添加图片",
                ["ja"] = "会話に画像を追加"
            },
            ["ParametresBoutonTooltip"] = new()
            {
                ["fr"] = "Ouvrir la fenêtre des paramètres",
                ["en"] = "Open the settings window",
                ["es"] = "Abrir la ventana de configuración",
                ["it"] = "Apri la finestra delle impostazioni",
                ["ru"] = "Открыть окно настроек",
                ["zh"] = "打开设置窗口",
                ["ja"] = "設定ウィンドウを開く"
            },
            ["PromptNom"] = new()
            {
                ["fr"] = "Entrez le nouveau nom :",
                ["en"] = "Enter the new name:",
                ["es"] = "Introduzca el nuevo nombre:",
                ["it"] = "Inserisci il nuovo nome:",
                ["ru"] = "Введите новое имя:",
                ["zh"] = "输入新名称：",
                ["ja"] = "新しい名前を入力してください："
            },
            ["TitreRenommage"] = new()
            {
                ["fr"] = "Renommer la conversation",
                ["en"] = "Rename conversation",
                ["es"] = "Renombrar conversación",
                ["it"] = "Rinomina conversazione",
                ["ru"] = "Переименовать разговор",
                ["zh"] = "重命名对话",
                ["ja"] = "会話の名前を変更"
            },
            ["ConfirmationSuppression"] = new()
            {
                ["fr"] = "Voulez-vous vraiment supprimer cette conversation ?",
                ["en"] = "Do you really want to delete this conversation?",
                ["es"] = "¿Realmente desea eliminar esta conversación?",
                ["it"] = "Vuoi davvero eliminare questa conversazione?",
                ["ru"] = "Вы действительно хотите удалить этот разговор?",
                ["zh"] = "您真的想删除此对话吗？",
                ["ja"] = "この会話を本当に削除しますか？"
            },
            ["TitreConfirmation"] = new()
            {
                ["fr"] = "Confirmation",
                ["en"] = "Confirmation",
                ["es"] = "Confirmación",
                ["it"] = "Conferma",
                ["ru"] = "Подтверждение",
                ["zh"] = "确认",
                ["ja"] = "確認"
            },
            ["ImageAjoutee"] = new()
            {
                ["fr"] = "Image ajoutée",
                ["en"] = "Image added",
                ["es"] = "Imagen agregada",
                ["it"] = "Immagine aggiunta",
                ["ru"] = "Изображение добавлено",
                ["zh"] = "图片已添加",
                ["ja"] = "画像が追加されました"
            },
            ["ImageInseree"] = new()
            {
                ["fr"] = "[Image insérée]",
                ["en"] = "[Image inserted]",
                ["es"] = "[Imagen insertada]",
                ["it"] = "[Immagine inserita]",
                ["ru"] = "[Изображение вставлено]",
                ["zh"] = "[图片已插入]",
                ["ja"] = "[画像が挿入されました]"
            }
        };

        public static string T(string key)
        {
            if (Textes.TryGetValue(key, out var traductions))
            {
                return traductions.TryGetValue(Langue, out var t) ? t : traductions["fr"];
            }
            return key;
        }
    }
}
