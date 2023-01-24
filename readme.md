# CelesteNyaNetBot

蔚蓝群服喵服的q群bot

相较于编译后的版本缺少`KeySettings.json`, 以下是该文件的格式(截止2023-1-24):

```json
{
  "TokenService": {
    "Session": "session",
    "BaseUrl": "http://后端的.地址"
  },
  "Connection": {
    "WebSocketUri": "ws://后端的.go-cqhttp的.ws地址"
  }
}
```