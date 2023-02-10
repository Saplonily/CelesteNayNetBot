namespace CelesteNyaNetBot;

public static class TipStrings
{
    public const string TipInternalError = "内部异常";
    public const string TipInternalErrorWithCode = "内部异常，消息为 {0}, {1}";

    public const string MarkerPrefixNotExists = "prefix not";
    public const string MarkerAccountNotBinded = "user not";
    public const string MarkerColorNotExists = "color not";

    public const string TipAccountNotBinded = "暂未绑定群服账号, 请先使用\"!bind 用户名\"指令绑定账号";

    public const string TipInvalidName = "昵称不符合规则！只允许大小写字母和数字以及符号_和^和.的组合.";
    public const string TipNameChanged = "你的名称已更改为 \"{0}\".";
    public const string TipNameChangeCooldowning = "改名冷却中！ 剩余时间: {0}";
    public const string TipNameChangeCooldowningTimeSpanFormat = @"d\天\ hh\:mm\:ss";

    public const string TipPrefixListingHead = "目前你拥有的头衔有:\n";
    public const string TipNotOwnedAnyPrefix = "你还暂未拥有任何头衔呢...";
    public const string TipPrefixReset = $"你的头衔已更换至默认头衔(无头衔).";
    public const string TipPrefixChanged = "你的头衔已更换至 \"{0}\".";
    public const string TipNotOwnedPrefix = "你并没有这个 \"{0}\" 头衔哦, 请检查拼写是否正确.";

    public const string TipNotOwnedAnyColor = "你还暂未拥有任何昵称颜色呢...";
    public const string TipColorListingHead = "目前你拥有的昵称颜色有:\n";
    public const string TipColorListingFormat = "#{0} - {1}\n";
    public const string TipColorListingTail = "tip: 使用\"!change_color <编号>\"以更改颜色.";
    public const string TipColorReset = $"你的昵称颜色已更换至默认颜色(#FFFFFF)";
    public const string TipColorChanged = "你的昵称颜色已更换至 \"{0}\".";
    public const string TipNotOwnedColor = "你并没有这个 \"{0}\" 颜色哦, 请检查拼写是否正确或者使用\"!get_colors\"指令查询所有可用颜色.";
    public const string TipColorIndexOutOfRange = "编号超出范围了! 请检查编号是否在\"!get_colors\"列表中.";

    public const string TipHelp = """
        --蔚蓝喵服群bot使用说明--
        1. 登录模块
          - !bind <用户名> 将你的qq账号绑定至<用户名>上
          - !bind <用户名> confirm 确认上面指令的执行
          - !relogin 重新获取登录凭证并过期之前的凭证
          - !change_name <用户名> 更改你的名字, 冷却时间28天

        2. 头衔、颜色更改模块
          - !get_colors 获取拥有的所有颜色
          - !change_color <颜色编号或者颜色hex> 将昵称颜色更改至目标颜色
          - !get_prefixes 获取拥有的所有头衔
          - !change_prefix <头衔> 更换为目标头衔

        3. FAQ
          Q: 注册的具体流程
          A: 请参阅置顶群公告
          Q: 我更换设备了或遗失了凭证后登录不上群服
          A: 可使用!relogin指令重新获取新的链接并过期旧链接
        """;
}
