# SUNIDRA2
SECCON 2016 × CEDEC CHALLENGEで扱われたゲームのソースコード  

[SECCON 2016 × CEDEC CHALLENGE 競技概要](http://2016.seccon.jp/schedule/seccon-2016-cedec.html)  
[SECCON 2016 x CEDEC CHALLENGE ゲームクラッキング＆チートチャレンジ](http://cedec.cesa.or.jp/2016/session/ENG/6536.html)  

[問題ファイル一式](http://1drv.ms/u/s!AtaQw-wbmFVpey-cWSc8e4RQJlQ)  

## How to play
1. [問題ファイル一式](http://1drv.ms/u/s!AtaQw-wbmFVpey-cWSc8e4RQJlQ) をダウンロード
2. NightmaresのSCORE, HPを任意に改ざん
3. NightmaresのGame DLL(/Data/Managed/Assembly-CSharp-*.dll)の復元


1. SUNIDRA2のaccount登録（https://cedec.seccon.jp/2016/sign-up）
2. SUNIDRA2のスタミナを任意に改ざん
3. SUNIDRA2のスタミナ以外のステータスを改ざん
4. SUNIDRA2の暗号化された通信プロトコルの解析

## How to build
[Unity](https://unity3d.com/jp/)をダウンロード、インストール。

ソースコードをcloneする。
```
git clone https://github.com/kenjiaiko/SUNIDRA2
```
Unityから開く。

File -> Build Settings（Build Settingsのウィンドウが開く）
Android -> Player Settings -> Other Settings -> Configuration -> Scripting Backend -> IL2CPP

Build

Build途中でAndroid SDKとNDKが求められるため、ダウンロード＆pathを指定。

## Reference
- [Unityゲーム開発-オンライン3Dアクションゲームの作り方](http://www.amazon.co.jp/dp/4797374403)
- [DOVA SYNDROME](http://dova-s.jp/)：[晴れた日の街（BGM）](http://dova-s.jp/bgm/play4595.html)、[1f（効果音）](http://dova-s.jp/se/play114.html)
- [FLAT ICON DESIGN（アイコン）](http://flat-icon-design.com/)
