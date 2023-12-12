# VRCUrlInputFieldResolver
VRCUrlInputFiledのMissingScriptを解決するツール。  
VRChatのワールド作成時などにご利用ください。

## 導入方法

VCCをインストール済みの場合、以下の**どちらか一つ**の手順を行うことでインポートできます。

- [VCC Listing](https://tp-jp.github.io/vpm-repos/) へアクセスし、「Add to VCC」をクリック

- VCCのウィンドウで `Setting - Packages - Add Repository` の順に開き、 `https://tp-jp.github.io/vpm-repos/index.json` を追加

[VPM CLI](https://vcc.docs.vrchat.com/vpm/cli/) を使用してインストールする場合、コマンドラインを開き以下のコマンドを入力してください。

```
vpm add repo https://tp-jp.github.io/vpm-repos/index.json
```

VCCから任意のプロジェクトを選択し、「Manage Project」から「Manage Packages」を開きます。
一覧の中から `VRCUrlInputFieldResolver` の右にある「＋」ボタンをクリックするか「Installed Vection」から任意のバージョンを選択することで、プロジェクトにインポートします。 

リポジトリを使わずに導入したい場合は [releases](https://github.com/tp-jp/vrc-url-input-field-resolver/releases) から unitypackage をダウンロードして、プロジェクトにインポートしてください。

## 使い方
シーンを開いた際に自動で処理が実行されるため、手動で実行する必要はありません。

## 更新履歴
[CHANGELOG](CHANGELOG.md)
