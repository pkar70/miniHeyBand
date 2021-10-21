' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SetDeviceGlobalParam
    Inherits Page

    ' bym tu dał:
    ' DegreeSwitchReq		19
    ' DisplayOrientationReq	29
    ' DisplayStyleReq		2a [read:1; write:2, index], com/oudmon/bandapi/req/DisplayStyleReq
    ' DndReq	(mute?)		 6
    ' DisplayClockReq		12
    ' MusicSwitchReq		1c
    'ReadPersonalizationSettingReq 17, 1, 2, 3, 0... (int mClockSetting, int mPowerOffSetting, int mPowerOnSetting)

    ' bez request, ale skądinąd wywolanie:
    ' FindDeviceReq		50++	55	aa	[ANTI_LOST_RATE]


    ' res\layout\activity_device_common.xml
    ' wspolne dla wszystkich:
    '<com..jxr202.colorful_ui.ItemView android:background="@drawable/bg_item_selector" android:focusable="true" android:clickable="true" android:layout_width="fill_parent" android:layout_height="55.0dip" jxr202:mLeftImage="@mipmap/ic_device_manager_flip" jxr202:mLeftTitleText="@string/flip_title" jxr202:mLeftTitleTextColor="#ff010101" jxr202:mLeftTitleTextSize="15.0sp" jxr202:mRightImage="@drawable/ic_item_more" jxr202:mRightImageVisibility="visible" jxr202:mRightTextColor="#ff999999" jxr202:mRightTextId="@id/flip_state" jxr202:mRightTextSize="14.0sp" jxr202:mRightTextVisibility="visible" jxr202:mBottomLineColor="#ffcccccc" />
    '<.ItemView id="@id/lost_settings"      mLeftImage="@mipmap/ic_device_manager_location" mLeftTitleText="@string/antio_lost" mRightImage="@drawable/ic_item_more" mRightImageVisibility="visible" mRightTextVisibility="gone"  />
    '<.ItemView id="@id/time_settings"      mLeftImage="@mipmap/ic_device_manager_time" mLeftTitleText="@string/time_mode" mRightTextId="@id/time_format"   />
    '<.ItemView id="@id/rate_settings"      mLeftImage="@mipmap/ic_device_rate_align" mLeftTitleText="@string/device_rate_title" mLeftTitleWidth="220.0dip" mRightImage="@drawable/ic_item_more" mRightImageVisibility="visible" mRightTextVisibility="gone"  />
    '<.ItemView id="@id/reset_settings"     mLeftImage="@mipmap/ic_device_manager_reset" mLeftTitleText="@string/device_reset_title" mRightImage="@drawable/ic_item_more" mRightImageVisibility="visible" mRightTextVisibility="gone"  />
    '<.ItemView id="@id/restore_settings"   mLeftImage="@mipmap/ic_device_restore" mLeftTitleText="@string/device_restore_title" mRightImage="@drawable/ic_item_more" mRightImageVisibility="visible" mRightTextVisibility="gone"  />
    '<.ItemView id="@id/we_chat_sport"      mLeftImage="@mipmap/we_chat_sport_logo" mLeftTitleText="@string/we_chat_sport" mRightImage="@drawable/ic_item_more" mRightImageVisibility="visible" mRightTextVisibility="gone"  />
    '(usuwalne) <.ItemView id="@id/weather_forecast"   mLeftImage="@mipmap/weather_forecast_logo" mLeftTitleText="@string/weather_forecast" mRightTextId="@id/forecast_state"   />
    '(usuwalne) <.ItemView id="@id/music_command"      mLeftImage="@mipmap/music_command_logo" mLeftTitleText="@string/music_command" mRightTextId="@id/music_command_state"   />

    'Lcom/oudmon/band/common/UIHelper;->showAntiLost(Landroid/content/Context;)V
    'Lcom/oudmon/band/common/UIHelper;->showTimeSetting(Landroid/content/Context;)V
    ' opis powyzszego jest tu: band/ui/activity/DeviceTimeActivity
    'Lcom/oudmon/band/common/UIHelper;->showReboot(Landroid/content/Context;)V
    'Lcom/oudmon/band/common/UIHelper;->showRestore(Landroid/content/Context;)V
    '' Lcom/oudmon/band/ui/activity/DeviceCommonActivity;->showRateDialog()V - to ocena thumbup/down
    'Lcom/oudmon/band/common/UIHelper;->showWeChatSport(Landroid/content/Context;)V
    'Lcom/oudmon/band/common/UIHelper;->showWeatherForecast(Landroid/content/Context;)V
    'Lcom/oudmon/band/common/UIHelper;->showMusicCommand(Landroid/content/Context;)V

    ' w initData jest sprawdzanie co jest supported:
    'invoke-static {}, Lcom/oudmon/band/models/DeviceFeatures;->isSupportWeatherForecast()Z
    'invoke-static {}, Lcom/oudmon/band/models/DeviceFeatures;->isSupportMusicCommand()Z

#Region "FanWan"
    ' Request:  PalmScreenReq		 5	 2	[1: enable, 2: disable], [b0: left, b1: right, b2: needpalm] RET: byte1,2 jak w request
    ' res\layout\activity_fanwan
    ' w band/ui/activity/DeviceCommonActivity processClick jest pokazywanie wybranego (czyli pewnie rozwijanie z listy)
    'invoke-static {p0}, Lcom/oudmon/band/common/UIHelper;->showFanWan(Landroid/content/Context;)V
    ' res\layout\activity_device_common.xml
    '<.ItemView id="@id/flip_settings"      mLeftImage="@mipmap/ic_device_manager_flip" mLeftTitleText="@string/flip_title" mRightTextId="@id/flip_state"   />

    Private Async Function readPalmScreenReq() As Task
        ' ustawienie UI po odczytaniu danych
        Dim aArr As Byte() = Await App.SendBandCommand(5, 5, 1)
        ' 5, 1, 1, 5, 0...

        If aArr(0) <> 5 Then Exit Function
        uiFanWanOnOff.IsOn = aArr(2) And 1
        uiFanWanRight.IsOn = Not (aArr(3) And 1)
        'uiFanWanThumb.IsOn = Not (aArr(3) And 4)

        uiFanWanOnOff.Visibility = Visibility.Visible
        uiFanWanRight.Visibility = Visibility.Visible
        'uiFanWanThumb.Visibility = Visibility.Visible
    End Function

    Private Async Function writePalmScreenReq() As Task
        ' PalmScreenReq(isEnable,isLeft, True)
        Dim c1 As Integer = If(uiFanWanOnOff.IsOn, 1, 0)
        Dim c2 As Integer = If(uiFanWanRight.IsOn, 0, 1)
        'If uiFanWanThumb.IsOn Then
        c2 = c2 Or 4     ' ale to jest niezmiennie TRUE :)
        ' end if 
        Dim aArr As Byte() = Await App.SendBandCommand(5, 5, 2, c1, c2) ' = 5,2,0,4
        Await readPalmScreenReq()
    End Function

#End Region

#Region "Time1224 i Unit"
    ' Request: TimeFormatReq		0A
    ' band/ui/activity/DeviceTimeActivity

    Private Async Function readTime1224() As Task
        Dim aArr As Byte() = Await App.SendBandCommand(5, &HA, 1)
        ' 10, 1, 0...
        uiTime1224.IsOn = Not aArr(2)
        uiImperialSI.IsOn = Not aArr(3)

        uiTime1224.Visibility = Visibility.Visible
        uiImperialSI.Visibility = Visibility.Visible
    End Function

    Private Async Function writeTime1224() As Task
        Dim c1 As Integer = If(uiTime1224.IsOn, 0, 1)
        Dim c2 As Integer = If(uiImperialSI.IsOn, 0, 1)
        Await App.SendBandCommand(5, &HA, 2, c1, c2)
    End Function
#End Region

#Region "ScreenSaver"
    ' Request: DisplayTimeReq		1f
    ' res\layout\activity_device_common.xml
    '(usuwalne) <.ItemView id="@id/display_time"       mLeftImage="@mipmap/display_time_logo" mLeftTitleText="@string/display_time" mRightTextId="@id/display_time_state"   />
    ' w band/ui/activity/DeviceCommonActivity processClick jest pokazywanie wybranego (czyli pewnie rozwijanie z listy)
    'invoke-static {p0}, Lcom/oudmon/band/common/UIHelper;->showDisplayTime(Landroid/content/Context;)V
    ' czyli dialog com/oudmon/band/R$layout;->activity_display_time ; display_time, display_time_tip: Wydłużenie czasu wyświetlania skróci żywotność baterii
    ' w initData jest sprawdzanie co jest supported:
    'invoke-static {}, Lcom/oudmon/band/models/DeviceFeatures;->isSupportDisplayTime()Z

    Private Async Function readScreenSaver() As Task
        Dim aArr As Byte() = Await App.SendBandCommand(5, &H1F, 1)
        If aArr(0) <> &H1F Then ' error = 0x1f | 0x80
            uiScreenSaver.Visibility = Visibility.Collapsed
            Exit Function
        End If
        uiScreenSaver.Value = aArr(2)   ' 159, 238, 0..
        uiScreenSaver.Visibility = Visibility.Visible
    End Function

    Private Async Function writeScreenSaver() As Task
        Dim c1 As Integer = uiScreenSaver.Value
        c1 = Math.Min(c1, 15)
        c1 = Math.Max(c1, 4)
        Await App.SendBandCommand(5, &H1F, 2, c1)
        Await readScreenSaver()
    End Function

#End Region

#Region "Brightness"
    ' Request: BrightnessSettingsReq	1b	[1:read, 2:write] [level]
    ' res\layout\activity_device_common.xml
    '(hidden) <.ItemView id="@id/brightness_settings" visibility="gone" mLeftImage="@mipmap/brightness_settings_logo" mLeftTitleText="@string/brightness_settings" mLeftTitleWidth="210.0dip" mRightImage="@drawable/ic_item_more" mRightImageVisibility="visible" mRightText="60%" mRightTextColor="#ff999999" mRightTextId="@id/brightness"   />
    ' w band/ui/activity/DeviceCommonActivity processClick jest pokazywanie wybranego (czyli pewnie rozwijanie z listy)
    'invoke-static {p0}, Lcom/oudmon/band/common/UIHelper;->showBrightnessSettings(Landroid/content/Context;)V

    Private Async Function readBrightness() As Task
        ' activity_brightness_settings.xml
        ' jasnosc jest w SeekBar

        Dim aArr As Byte() = Await App.SendBandCommand(5, &H1B, 1) ' 155, 238, 0...
        If aArr(0) <> &H1B Then ' error = 0x1b | 0x80
            uiBrightness.Visibility = Visibility.Collapsed
            Exit Function
        End If

    End Function

    Private Async Function writeBrightness() As Task
        Dim c1 As Integer = uiBrightness.Value
        c1 = Math.Min(c1, 9)
        c1 = Math.Max(c1, 0)
        Await App.SendBandCommand(5, &H1B, 2, c1)
        Await readBrightness()
    End Function
#End Region

#Region "bakteryjka"
    Private Async Function readPower() As Task
        Dim aArr As Byte() = Await App.SendBandCommand(5, 3)
        If aArr(0) <> 3 Then
            Exit Function
        End If
        uiPower.Value = aArr(1)
        If aArr(1) < 10 Then
            uiPower.Foreground = New SolidColorBrush(Windows.UI.Colors.Red)
        ElseIf aArr(1) < 25 Then
            uiPower.Foreground = New SolidColorBrush(Windows.UI.Colors.Orange)
        Else
            uiPower.Foreground = New SolidColorBrush(Windows.UI.Colors.Blue)
        End If

        App.AppendLog("connect", "Power", aArr(1) & " %")
        uiPower.Visibility = Visibility.Visible
    End Function

#End Region

    Private Async Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        Await readPower()
        Await readPalmScreenReq()
        Await readTime1224()
        'Await readScreenSaver() ' tego moze nie byc! 
        'Await readBrightness() ' tego aplikacja i tak nie pokazuje :)
        uiOk.Visibility = Visibility.Visible
    End Sub

    Private Async Sub uiFanWan_Clicked(sender As Object, e As RoutedEventArgs)
        Await writePalmScreenReq()
        Await writeTime1224()
        If uiBrightness.Visibility = Visibility.Visible Then Await writeBrightness()
        If uiScreenSaver.Visibility = Visibility.Visible Then Await writeScreenSaver()
    End Sub
    
End Class

