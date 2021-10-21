' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.ApplicationModel.Background
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SetWspolpraca
    Inherits Page

    Private Async Sub uiSendMsg_Click(sender As Object, e As RoutedEventArgs)
        Dim iType As Integer = uiMsgType.Text
        Dim sMsg As String = uiMessage.Text

        Await App.SendBandMsg(iType, sMsg)
    End Sub

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        'uiNotifySMS.IsOn = App.GetSettingsBool("notifySMS")
        uiTimerTetno.IsOn = App.GetSettingsBool("timerTetno")
        uiNotifyPomiar.IsOn = App.GetSettingsBool("notifyPomiar")
        uiNotifyBegin.Value = App.GetSettingsInt("notifyBegin", 9)
        uiNotifyEnd.Value = App.GetSettingsInt("notifyEnd", 20)
        uiNotifyBegin.IsEnabled = uiNotifyPomiar.IsOn
        uiNotifyEnd.IsEnabled = uiNotifyPomiar.IsOn
        uiTimerTime.IsOn = False
        If App.GetSettingsInt("timerEvery", 30) = 15 Then uiTimerTime.IsOn = True
        uiSyncClock.IsOn = App.GetSettingsBool("syncClock", False)

        Dim oBAS As BackgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync()
        If oBAS = BackgroundAccessStatus.AlwaysAllowed Or oBAS = BackgroundAccessStatus.AllowedSubjectToSystemPolicy Then
            '  uiNotifySMS.IsEnabled = True
            uiTimerTetno.IsEnabled = True
        Else
            ' uiNotifySMS.IsEnabled = False
            uiTimerTetno.IsEnabled = False
        End If

        If App.IsThisMoje Then uiWysylanieTekstow.Visibility = Visibility.Visible
    End Sub

    Private Async Sub uiOk_Click(sender As Object, e As RoutedEventArgs)
        'App.SetSettingsBool("notifySMS", uiNotifySMS.IsOn)
        App.SetSettingsBool("timerTetno", uiTimerTetno.IsOn)
        App.SetSettingsBool("notifyPomiar", uiNotifyPomiar.IsOn)
        App.SetSettingsInt("notifyBegin", uiNotifyBegin.Value)
        App.SetSettingsInt("notifyEnd", uiNotifyEnd.Value)
        App.SetSettingsInt("timerEvery", If(uiTimerTime.IsOn, 15, 30))
        App.SetSettingsBool("syncClock", uiSyncClock.IsOn)

        'App.IntLogSetLevel(2)

        Dim oBAS As BackgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync()

        If oBAS = BackgroundAccessStatus.AlwaysAllowed Or oBAS = BackgroundAccessStatus.AllowedSubjectToSystemPolicy Then
            ' usun wszystkie
            For Each oTask As KeyValuePair(Of Guid, IBackgroundTaskRegistration) In BackgroundTaskRegistration.AllTasks
                If oTask.Value.Name.IndexOf("PKARminiHejBand_SMS") > -1 Then oTask.Value.Unregister(True)
                If oTask.Value.Name.IndexOf("PKARminiHejBand_TimerTetno") > -1 Then oTask.Value.Unregister(True)
            Next

            Dim builder As BackgroundTaskBuilder = New BackgroundTaskBuilder
            Dim oRet As BackgroundTaskRegistration

            ' powlaczaj te, ktore sa włączone
            'If uiNotifySMS.IsOn Then
            '    Dim oTrigg As SystemTrigger = New SystemTrigger(SystemTriggerType.SmsReceived, False)
            '    builder.SetTrigger(oTrigg)
            '    builder.Name = "PKARminiHejBand_SMS"
            '    oRet = builder.Register()
            'End If


            'If uiTimerTetno.IsOn Then
            '    Dim oTrigg = New TimeTrigger(15, False)
            '    builder.SetTrigger(oTrigg)
            '    builder.Name = "PKARminiHejBand_TimerTetno"
            '    oRet = builder.Register()
            '    App.IntLogAppend(2, "timer tetno added")
            'End If

        End If


    End Sub

    Private Sub uiNotifyPomiar_Toggled(sender As Object, e As RoutedEventArgs) Handles uiNotifyPomiar.Toggled
        uiNotifyBegin.IsEnabled = uiNotifyPomiar.IsOn
        uiNotifyEnd.IsEnabled = uiNotifyPomiar.IsOn

    End Sub

    ' bym tu dał:
    ' [check] autoset time przy bind
    ' wybor komendy do wykonania na sygnal kamerkowania (camera / voice recorder)
    ' wysylanie powiadomien na zegarek - z SMS, kalendarza, poczty, telefon, ...
    ' alarm znikania zegarka z Bluetooth (w tekstach ze 5 m, 10 m?)
    ' wibracja moze tu? bo tu mniej :)

    ' WeatherForecastReq	1a
    ' ?? SetANCSReq		60
    ' ?? BindAncsReq		 4	2	0	0	0	0	0	0	0	0	0	0	0	0	0
    ' CameraReq		 2
    ' IntellReq		 9

    ' PushMsgUintReq		72


End Class
