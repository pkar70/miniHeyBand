' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

Imports System.Net.Http
Imports Windows.Data.Json
Imports Windows.Security.Cryptography
Imports Windows.Security.Cryptography.Core
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

#Region "komunikacja HTTP"

    Private Async Sub CheckZegarekFeatures()
        '  T93proB6DC , T93_V1.0 , T93_1.00.26_170601
        '  T93proB6DC , T93_V1.0 , T93_1.01.55_171130

        If Not App.IsNetIPavailable(True) Then Exit Sub

        Dim oJSON As JsonObject = New JsonObject
        Dim oVal2 As JsonValue = JsonValue.CreateStringValue("T93_V1.0")
        Dim oVal3 As JsonValue = JsonValue.CreateStringValue("T93_1.01.55_171130")
        oJSON.Add("hardware-version", oVal2)
        oJSON.Add("rom-version", oVal3)
        Dim sTxt As String = oJSON.ToString
        sTxt = Await OkHttpPost("device/features/list", sTxt)   ' error, byc moze potrzeba login?

        App.DialogBox("Trying to CheckZegarekFeatures:" & vbCrLf & sTxt)

    End Sub
    Private Async Function CheckHeyBandUpdate(Optional pForceCheck As Boolean = False) As Task

        Dim iToday As Integer = App.CheckDaysPassed("lastHeyBandcheck", 7)
        If Not pForceCheck Then
            If iToday = 0 Then Exit Function
        End If

        If Not App.IsNetIPavailable(True) Then Exit Function

        Dim oJSON As JsonObject = New JsonObject
        Dim oVal1 As JsonValue = JsonValue.CreateNumberValue(&H17)
        Dim oVal2 As JsonValue = JsonValue.CreateStringValue("android")

        oJSON.Add("app-id", oVal1)
        oJSON.Add("type", oVal2)
        Dim sTxt As String = oJSON.ToString
        'sTxt = "{""app-id"":23,""type"":""android""}"

        ' "(current: {""app-id"":23,""download-url"":""http://qn-download.jimyun.com/app_id_23/1527851124354/HeyBand1.30.07.apk"",""type"":""android"",""curr"":""3007"",""limit"":"""",""desc"":""""})"
        sTxt = Await OkHttpPost("app-update/version-info", sTxt)

        If sTxt.IndexOf("HeyBand1.30.07.apk") > 0 Then Exit Function
        App.ClipPut(sTxt)
        App.SetSettingsInt("lastHeyBandcheck", iToday)

        oJSON = JsonObject.Parse(sTxt)
        ' curr, limit, desc, download-url
        Dim sFile As String = oJSON.GetNamedString("download-url")
        Dim iInd As Integer = sFile.LastIndexOf("/")
        If iInd > 0 Then sFile = sFile.Substring(iInd + 1)
        uiHBappWeb.Text = "(Web: " & oJSON.GetNamedString("curr") & ", " & sFile & ")"
        App.DialogBox("Newer version:" & vbCrLf & sTxt)

    End Function
    Public Async Function OkHttpPost(sUrlPart As String, Optional sJSON As String = "") As Task(Of String) ' oraz callback

        App.moHttp.DefaultRequestHeaders.Add("jim-app-id", "iu3TKjwRUCGfIwtTH9gXeYsq")
        App.moHttp.DefaultRequestHeaders.Add("app-version", "0")
        App.moHttp.DefaultRequestHeaders.Add("Accept-Language", "en")
        App.moHttp.DefaultRequestHeaders.Add("accept-language", "en")

        Dim d1970 As DateTime = New DateTime(1970, 1, 1, 0, 0, 0)
        Dim iTime As Long = (DateTime.Now.ToUniversalTime - d1970).TotalMilliseconds
        Dim sTime As String = iTime.ToString
        Dim v12 As String = "kJek81coyFG4V3eSg79b82HU" & sTime

        Dim strAlgName As String = HashAlgorithmNames.Md5
        Dim buffUtf8Msg As Windows.Storage.Streams.IBuffer = CryptographicBuffer.ConvertStringToBinary(v12, BinaryStringEncoding.Utf8)
        Dim oAlg As HashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(strAlgName)
        Dim buffHash As Windows.Storage.Streams.IBuffer = oAlg.HashData(buffUtf8Msg)
        Dim sMd5 As String = CryptographicBuffer.EncodeToHexString(buffHash)

        App.moHttp.DefaultRequestHeaders.Add("jim-app-sign", sMd5 & "," & sTime)

        sUrlPart = "app-update/version-info"
        Dim sUri As String = "https://api.jimyun.com/v1/" & sUrlPart


        Dim oHttpCont As StringContent = New StringContent(sJSON, Text.Encoding.UTF8, "application/json")

        Dim oRes As HttpResponseMessage = Await App.moHttp.PostAsync(sUri, oHttpCont)

        Return Await oRes.Content.ReadAsStringAsync
    End Function

#End Region

    Private Async Function CheckFirmwareUpdate(bForceCheck As Boolean) As Task

        Dim iToday As Integer = App.CheckDaysPassed("lastFirmwareCheck", 7)
        If Not bForceCheck Then
            If iToday = 0 Then Exit Function
        End If

        If Not Await App.IsNetBTavailable(True) Then Exit Function
        If Not App.IsNetIPavailable(True) Then Exit Function

        Dim oJSON As JsonObject = New JsonObject

        oJSON.Add("device-type", JsonValue.CreateStringValue("Hey_BAND"))
        Dim sTxt As String = App.GetSettingsString("bindedToName")
        oJSON.Add("device-id", JsonValue.CreateStringValue(sTxt))

        Dim sSvc As String = "0000180A-0000-1000-8000-00805F9B34FB"
        Dim sTmp As String = Await App.ReadBLEchars(sSvc, "2A27", True)
        If sTmp <> "" Then
            oJSON.Add("hardware-version", JsonValue.CreateStringValue(sTmp))
            oJSON.Add("rom-version", JsonValue.CreateStringValue(Await App.ReadBLEchars(sSvc, "2A26", True)))
            ' "{""device-type"":""Hey_BAND"",""device-id"":""T93proB6DC"",""hardware-version"":""T93_V1.0"",""rom-version"":""T93_1.00.26_170601""}"
            sTxt = Await OkHttpPost("device-info/update", oJSON.ToString)
            App.ClipPut(sTxt)
            App.SetSettingsInt("lastFirmwareCheck", iToday)
            App.DialogBox("Newer FW version:" & vbCrLf & sTxt)
            ' {"message":"wrong data in field :type, require-type : required, data :"}
        End If

        ' tu moze byc: jesli jest zmiana na nowszy, to pokazanie roznic miedzy features
    End Function

    'E:\NoweDVD\Public\Programs\dex2jar

    Private Async Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)

        If App.moHttp Is Nothing Then App.moHttp = New HttpClient

        If App.IsThisMoje() Then
            uiHBappVers.Text = "(made from 1.30.07)"    ' 1.30.07, z 1 czerwca
            Await CheckHeyBandUpdate(False)
            uiHBappVers.Visibility = Visibility.Visible
        Else
            uiHBappVers.Visibility = Visibility.Collapsed
        End If

        App.RegisterTriggers()  ' zacznij uruchamiac co godzine, trafisz na polnoc

        Dim sTxt As String = App.GetSettingsString("bindedToName")
        If sTxt <> "" Then
            sTxt = "(using band " & sTxt & ","
            If uiGrid.ActualWidth < 400 Then
                sTxt = sTxt & vbCrLf
            Else
                sTxt = sTxt & " "
            End If
            sTxt = sTxt & "addr: " & App.GetSettingsString("bindedToAddrDisplay") & ")"
            uiUsingBand.Text = sTxt

            'Await App.BindMoBleAsync(App.GetSettingsString("bindedToAddr"))

            'Await CheckFirmwareUpdate(False) ' request signature error?
            'CheckZegarekFeatures() ' request signature error?
        End If

        '        uiInternalLog.Text = App.GetSettingsString("internalog")

        ' CheckZegarekFeatures()
    End Sub

    Private Sub uiBind_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(BindBand))
    End Sub
    Private Function NotBinded() As Boolean
        If App.GetSettingsString("bindedToName") <> "" Then Return False

        App.DialogBoxRes("errorNotBinded")
        Return True
    End Function

    Private Sub uiSetClock_Tapped(sender As Object, e As TappedRoutedEventArgs)
        If NotBinded() Then Exit Sub
        Me.Frame.Navigate(GetType(SetDeviceClock))
    End Sub

    Private Async Sub uiVibrate_Tapped(sender As Object, e As TappedRoutedEventArgs)
        ' .class public Lcom/oudmon/bandapi/req/FindDeviceReq;
        ' com/oudmon/band/ui/activity/DeviceLocationActivity;
        ' com/oudmon/band/ui/activity/DeviceLocationActivity$3;

        ' aczkolwiek tam jest z czekaniem na odpowiedz
        If NotBinded() Then Exit Sub

        Dim aArr As Byte() = Await App.SendBandCommand(1, &H50, &H55, &HAA)
        If aArr Is Nothing Then Exit Sub ' 20180811, gdy wraca Null
        If aArr(0) = &H80 Then Exit Sub
        App.DialogBox("Error? Have response, but unexpected")

        ' powinno byc: 16 bajtów, [0]=0x80, [1..14]=0, [15]=0x80 (=crc)
    End Sub

    Private Sub uiCommon_Tapped(sender As Object, e As TappedRoutedEventArgs)
        If NotBinded() Then Exit Sub

        Me.Frame.Navigate(GetType(SetDeviceGlobalParam))
    End Sub

    Private Sub uiZdrowie_Tapped(sender As Object, e As TappedRoutedEventArgs)
        If NotBinded() Then Exit Sub

        Me.Frame.Navigate(GetType(SetZdrowie))
    End Sub

    Private Sub AppBarButton_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(LogViewer))
    End Sub

    Private Sub uiClock_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(TimerOnOff))
    End Sub

    Private Sub uiWspolpraca_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Me.Frame.Navigate(GetType(SetWspolpraca))
    End Sub
End Class
