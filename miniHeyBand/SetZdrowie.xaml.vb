' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SetZdrowie
    Inherits Page


    ' kroki, cisnienie, puls, tlen, sen, kcal, km
    'a skad sie bierze fatigue?
    ' ważne: ciśnienie, puls, tlen

    'BpSettingReq		0c (isEnable, startEndTimeEntity, multiple)
    'HeartRateSettingReq	16	 (1=read/2=write, wtedy 2=!isEnable, 1=isEnable) , odczyt: MainDeviceFragment$7;
    'ReadBandSportReq	13	(long time)
    'ReadTotalSportDataReq	 7	(theDayOffset)
    'StartHeartRateReq	69, 1; callback: ui/activity/RealTimeRateActivity$3
    'StopHeartRateReq	6a, type, bb, cc
    '  CMD_BP_TIMING_MONITOR_DATA As Byte = 0xdt	' jest samodzielne w HealthHomeActivity.smali
    '  CMD_CALIBRATION_RATE As Byte = 0x20t	' jest samodzielne w DeviceRateAlignActivity

    ' isSupportBloodPressure	' not used
    ' isSupportHeartRatePlus	' w wielu miejscach
    ' isSupportRegularlyBloodPressure ' w band/ui/fragment/MainDeviceFragment$8$1;
    ' isSupportSport		' w wielu
#Region "wymuszanie pomiaru tetna"
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\
    ' smali_classes2\com\oudmon\band\ui\fragment\HealthRateFragment.smali
    '.method private startMeasureRate()V
    'invoke-static {v3=1}, Lcom/oudmon/bandapi/req/StartHeartRateReq;->getSimpleReq(B)Lcom/oudmon/bandapi/req/StartHeartRateReq;
    'invoke-direct {v2, p0}, Lcom/oudmon/band/ui/fragment/HealthRateFragment$5;-><init>(Lcom/oudmon/band/ui/fragment/HealthRateFragment;)V
    'invoke-virtual {v0, v1, v2}, Lcom/oudmon/bandapi/OdmHandle;->executeReqCmd(Lcom/oudmon/bandapi/req/BaseReqCmd;Lcom/oudmon/bandapi/IOdmOpResponse;)V
    ' const/16 v0, 0x1e
    ' iput v0, p0, Lcom/oudmon/band/ui/fragment/HealthRateFragment;->mCountDown:I
    ' ze niby na HealthRateFragment$5 jest tylko zerowanie mRealTimeValues

    ' bandapi/rsp/StartHeartRateRsp kolejne bajty: type, errcode, value
    ' if status<>0 then exit
    ' If type <>0 then exit 
    ' If errcode = 2 then exit 'no wearing'
    '
    ' StartHeartRateReq.getRealtimeHeartRate(action) -> init(6,action)
    ' StartHeartRateReq.getSimpleReq(type) -> init(type,action=0,1,2; od 3: 0x25)
    ' StartHeartRateReq.<init>(type,sub)
    ' request: [*]=key=0x69 [0]=type, [1]=sub
    '.field public static final ACTION_CONTINUE:B = 0x3t
    '.field public static final ACTION_PAUSE:B = 0x2t
    '.field public static final ACTION_START:B = 0x1t
    '.field public static final ACTION_STOP:B = 0x4t
    '.field public static final TYPE_BLOODOXYGEN:B = 0x3t   countdown=30
    '.field public static final TYPE_BLOODPRESSURE:B = 0x2t countdown=30
    '.field public static final TYPE_FATIGUE:B = 0x4t       countdown=30
    '.field public static final TYPE_HEALTHCHECK:B = 0x5t
    '.field public static final TYPE_HEARTRATE:B = 0x1t     countdown=30
    '.field public static final TYPE_REALTIMEHEARTRATE:B = 0x6t

    ' StopHeartRateReq.init(type,b,c) -> żądanie 6a, type, b,c
    ' stopBloodOxygen(oxyvalue) -> init(3,oxyvalue,0)
    ' stopBloodPressure(sbp,dbp) -> init(2,sbp,dbp)
    ' stopFatigue(fatigueScore) -> init(4,fatigueScore,0)
    ' stopHealthCheck() -> init(5,0,0)
    ' stopHeartRate(hrValue) -> init(1,hrValue,0)

    Private Async Function measureSomethingWait(iWait As Integer, uiText As TextBlock, sPrefix As String) As Task

        For i As Integer = iWait To 1 Step -1
            If uiText IsNot Nothing Then uiText.Text = sPrefix & " please wait " & i & " sec"
            Await Task.Delay(1000)
        Next
    End Function

    Private Async Function startMeasuringRTTetno() As Task

        If Not Await App.measureSomethingStart(6) Then Exit Function

        Await measureSomethingWait(10, uiStartTetno, "Tetno: ")

        Await App.measureSomethingLoopNew(200, 0, 10, uiStartTetno, "Tetno: ")

        App.measureSomethingStop()

    End Function
#If False Then

    Private Async Function startMeasuringTetno() As Task
        If Not Await App.measureSomethingStart(1) Then Exit Function

        Await App.measureSomethingLoop(200, 0, uiStartTetno2, "Tetno: ")

        App.measureSomethingStop()

    End Function

    Private Async Function startMeasuringTlen() As Task
        If Not Await App.measureSomethingStart(3) Then Exit Function

        Await App.measureSomethingLoop(200, 0, uiStartTlen, "Tlen: ")

        App.measureSomethingStop()

        '.class public Lcom/oudmon/band/ui/fragment/HealthOxygenFragment;
        ' invoke-static {v1=3}, Lcom/oudmon/bandapi/req/StartHeartRateReq;->getSimpleReq(B)Lcom/oudmon/bandapi/req/StartHeartRateReq;
        ' listener: new-instance v2, Lcom/oudmon/band/ui/fragment/HealthOxygenFragment$5;

    End Function

    Private Async Function startMeasuringFatyga() As Task
        If Not Await App.measureSomethingStart(4) Then Exit Function

        Await App.measureSomethingLoop(200, 0, uiStartFatyga, "Fatyga: ")

        App.measureSomethingStop()
        'fatyga:
        '0..0x14
        '28..3c
        '50..64
    End Function

    Private Async Function startMeasuringPressure() As Task
        If Not Await App.measureSomethingStart(2) Then Exit Function

        Await App.measureSomethingLoop(200, 0, uiStartPressure, "Cisnienie: ")

        App.measureSomethingStop()

    End Function

    Private Async Function startMeasuringHealth() As Task
        If Not Await App.measureSomethingStart(5) Then Exit Function

        Await App.measureSomethingLoop(200, 0, uiStartHealth, "Zdrowie: ")

        App.measureSomethingStop()

    End Function
#End If

#End Region
#Region "SportData"
    'ReadDetailSportDataReq	43	(int dayoffset, int startindex, int endindex) - response: kalorie, kroki, dystans, data
    ' powinno niby byc tak jak dane snu, ktore działają - czyli z danymi historycznymi
    ' 
    Private Async Function readKrokiHistorycznie() As Task
        uiKrokiHistoria.Text = "reading steps history ..."
        Dim sTxt As String = ""
        For i As Integer = 0 To &H50
            Dim aArr As Byte() = Await App.SendBandCommand(5, &H43, i, 0, &H5F)
            If aArr(0) <> &H43 Then Exit For
            If aArr(1) = &HFF Then Exit For
            'sTxt = sTxt & "20" & App.deBCD(aArr(1)) & "." & App.deBCD(aArr(2)) & "." & App.deBCD(aArr(3)) & ": "
            'sTxt = sTxt & "(" & aArr(4) & ", " & aArr(5) & ", " & aArr(6) & ") "
            'sTxt = sTxt & aArr(7) & ", " & aArr(8) & ", " & aArr(9) & ", " & aArr(10) & ", " & aArr(11) & ", " & aArr(12) & ", " & aArr(13) & ", " & aArr(14) & vbCrLf
        Next
        If sTxt <> "" Then
            sTxt = "Steps history" & vbCrLf & sTxt
        Else
            sTxt = "(StepsHistory unsupported or empty)"
        End If
        uiKrokiHistoria.Text = sTxt
        ' 0,0,5f -> &h44; tak samo dla 1,0,5f.

    End Function

#End Region
#Region "dane snu"
    'ReadSleepDetailsReq	44	(int dayoffset, int startindex, int endindex)  ' przy czytaniu: ind, 0, 0x5f
    ' HeySleepFragment: obtainSomeDaySleep(int dayIndex) {ReadSleepDetailsReq(dayIndex, 0, 0x5f), callback band/ui/fragment/HeySleepFragment$5)
    ' dayIndex zjezdza w dół do zera, zmienna: mSleepIndex
    ' ustawiany przez access$1202(x0, int day)

    ' HeySleepFragment$SaveBandSleepTask.smali:
    ' v8 = qual(1)
    ' If v8 >= 10 GoTo cond0
    ' If <0 goto cond0

    'v8= qual(1)
    'v8*=100 000
    'v10=qual(2)
    'v10*=1000
    'v8 +=v10

    'v10=qual(3)
    'v4= v8+v10
    Private Async Function readSen() As Task
        uiSen.Text = "reading sleep details..."
        Dim sTxt As String = ""
        For i As Integer = 0 To &H1D
            Dim aArr As Byte() = Await App.SendBandCommand(5, &H44, i, 0, &H5F)
            If aArr Is Nothing Then Exit For
            If aArr(0) <> &H44 Then Exit For
            If aArr(1) = &HFF Then Exit For
            Dim sDate As String = "20" & App.deBCD(aArr(1)) & "." & App.deBCD(aArr(2)) & "." & App.deBCD(aArr(3))
            If sDate < App.GetSettingsString("logLastSleepData", "1999") Then Exit For  ' nie pokazuj tu danych, ktore są zapisane w logu
            sTxt = sTxt & sDate & ": "
            sTxt = sTxt & "(" & aArr(4) & ", " & aArr(5) & ", " & aArr(6) & ") "
            sTxt = sTxt & aArr(7) & ", " & aArr(8) & ", " & aArr(9) & ", " & aArr(10) & ", " & aArr(11) & ", " & aArr(12) & ", " & aArr(13) & ", " & aArr(14) & vbCrLf
            uiSen.Text = "SleepDetails" & vbCrLf & sTxt
        Next
        If sTxt <> "" Then
            sTxt = "SleepDetails" & vbCrLf & sTxt
        Else
            sTxt = "(SleepDetails unsupported or empty)"
        End If
        uiSen.Text = sTxt

        App.AppendLog("sleep", "", sTxt)
        ' 0,0,5f -> &h44; tak samo dla 1,0,5f.

    End Function

    ' kolejne dni zapisane, wywolywane z kolejnymi indeksami
    ' response(iInd)
    ' meaning   1       2       3       4       5
    '  cmd      0x44    =       =       =       =
    '  year     0x18    =       =       =       0xff [=end]
    '  month    6       6       6       6       0
    '  day      9       8       7       6       0
    '  index    0       42      78      89
    '  ?        0       8       19      12
    '  ?        1       44      31      15
    ' qual(0)   2       5       5       5       ' moze sleep?
    ' qual(1)   9       15      15      15      ' moze deep?
    ' qual(2)   70      70      30      30      ' moze shallow?
    ' qual(3.) 100      0       0       100     ' moze wakecount?
    ' a po nocy 8/9 VI 2018, leżąc na stole :)
    ' total: 5:23, plytki 3:00, gleboki 2:30, rysunek roznej grubosci i wysokosci kreski
    'SleepDetails
    '2018.6.9: (0, 0, 1) 2, 9, 70, 100, 100, 100, 100, 100
    '2018.6.8: (41, 7, 44) 5, 15, 40, 0, 0, 0, 0, 0
    '2018.6.7: (33, 6, 31) 5, 15, 30, 0, 0, 0, 0, 0
    '2018.6.6: (91, 14, 15) 4, 9, 80, 100, 100, 100, 100, 100
    ' czyli ze sie zmieniają te parametery??

    '<String name = "average_awake_up" > Codzienny czas budzenia</String>
    '<String name = "average_deep_sleep" > Daily Deep Sleep</String>
    '<String name = "average_fall_sleep" > Dzienny czas snu</String>
    '<String name = "average_label" > średnia</String>
    '<String name = "average_shallow_sleep" > dzienny czas snu</String>
    '<String name = "average_sleep" > dzienny czas snu</String>
    '<String name = "awake_up_label" > czas wybudzania</String>
    ' w:
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\fragment_sleep_month_history.xml
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\fragment_sleep_week_history.xml

    '<String name = "fall_sleep_label" > czas snu</String>
    '<String name = "fall_time_off1" >• stosować się do harmonogramu, jeśli wstać późno rano, To możesz spać w nocy.</String>
    '<String name = "fall_time_off2" >• Trzymaj się z dala od kawy i nikotyny przed pójściem spać. Sugeruję, żebyś nie piła kawy na osiem godzin przed pójściem spać.</String>
    '<String name = "fall_time_on1" >• odpowiedni sen jest zapewnienie sprawnego przebiegu podstawie metabolizmu organizmu, będąc w stanie zagwarantować następny dzień pełen energii. Zaleca się, aby warunki pozwalały, o ile To możliwe, na zasypianie na czas, znacznie złagodziły fizyczne i psychiczne zmęczenie.</String>
    '<String name = "fall_time_on2" >• Zachowaj spokój i wyłącz telewizor i telefon, ponieważ cisza jest dobra dla poprawy jakości twojego snu.</String>
    ' w:
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepDayFragment.smali
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepMonthFragment.smali
    'E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepWeekFragment.smali

    '<String name = "hey_sleep_deep_label" > czas głębokiego snu</String>
    '<String name = "hey_sleep_quality_level1" > słaba jakość snu</String>
    '<String name = "hey_sleep_quality_level2" > jakość snu</String>
    '<String name = "hey_sleep_quality_level3" > jakość snu</String>
    '<String name = "hey_sleep_quality_level4" > Jakość snu</String>
    '<String name = "hey_sleep_shallow_label" > krótki czas snu</String>
    '<String name = "hey_sleep_sleep_label" > czas snu</String>
    '<String name = "hey_sleep_sleep_record" > zeszły sen</String>
    '<String name = "hey_sleep_tab" > sleep</String>
    ' w:
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\fragment_hey_sleep.xml
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\fragment_sleep_day_history.xml


    '<String name = "no_sleep_data1" >• Właściwa sen jest zapewnienie sprawnego przebiegu podstawie metabolizmu organizmu, będąc w stanie zagwarantować następny dzień pełen energii. Zaleca się zachowanie nawyku spania, wczesne wstawanie i wczesne wstawanie.</String>
    '<String name = "no_sleep_data2" >• Zbyt mało snu i zbyt wiele wpłynie na ich zdrowie, w porównaniu Do około ośmiu godzin snu, spać mniej niż ryzyko cierpiących na otyłość, która jest około 2 razy więcej czasu snu cierpi na otyłość Ryzyko jest około 1,5 razy większe. Więc proszę o rozsądny czas, aby zapewnić czas snu i jakość.</String>
    '<String name = "no_sleep_data3" >• uśpienia można uzupełnić energię organizmu, wzmacniają odporność oraz promowanie normalnego wzrostu i rozwoju organizmu ludzkiego, ciało uzyskać wystarczająco dużo odpoczynku. Tak więc wystarczająca ilość snu i dobra jakość snu jest ważnym warunkiem zdrowia.</String>
    '<String name = "no_sleep_data4" >• 23:00 codziennie przed pójściem spać, organizm może promować normalne detoksykacji wątroby, w celu zapewnienia normalnego funkcjonowania funkcji organizmu.</string>
    ' w:
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepDayFragment.smali
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepMonthFragment.smali
    'E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepWeekFragment.smali

    '<String name = "sleep_deep_label" > deep sleep: %1$s</string>
    '<String name = "sleep_end" >End</string>
    '<String name = "sleep_health_off1" >• Nie należy jeść ani pić gwałtownie przed pójściem do łóżka. Około dwóch godzin, aby zjeść mały posiłek przed snem, nie pić zbyt dużo wody, ponieważ toalety w nocy nadal będzie wpływać na jakość snu, noc nie jeść pikantnych potraw bogatych w tłuszcze, ponieważ te pokarmy wpływają również sen.</String>
    '<String name = "sleep_health_off2" >• Trzymaj się z dala od kawy i nikotyny przed pójściem spać. Sugeruję, żebyś nie piła kawy na osiem godzin przed pójściem spać.</String>
    '<String name = "sleep_health_on1" >• Powolne tempo oddychania przed snem. Przed snem można prawidłowo siedzieć, chodzić, oglądać w zwolnionym tempie telewizji, słuchanie muzyki i innych nisko i ciała stopniowo w cichej, spokojnej rodzi Yin Yin Sheng spać spokojnie, najlepszym sposobem jest zrobić wycieczkę w łóżku kilka minut qigong statycznej Aby utrzymać ducha, spać, jakość snu będzie najlepsza.</String>
    '<String name = "sleep_health_on2" >• Zanim pójdziesz spać, weź kąpiel. Gorąca kąpiel przed snem może pomóc rozluźnić mięśnie i sprawić, że będziesz lepiej spać.</String>
    '<String name = "sleep_history_advice" > sugestie dotyczące uśpienia</String>
    '<String name = "sleep_history_title" > historia snu</String>
    '<String name = "sleep_shallow_label" > light sleep: %1$s</string>
    '<String name = "sleep_start" > Start</String>
    '<String name = "sleep_time_off1" >• Sleep Staraj się nie polegać na pigułkach nasennych. Przed podjęciem pigułki nasenne należy skonsultować się z lekarzem, proponuję wziąć tabletki nasenne mniej niż 4 tygodnie.</String>
    '<String name = "sleep_time_off2" >• oczyścić spraye pokój i chemicznych środków czyszczących stosowanych mogą podrażniać drogi oddechowe, co wpływa na sen, zaleca się jedynie oczyścić sypialni rano.</String>
    '<String name = "sleep_time_on1" >• Zdrzemnij się w nocy. Picie w ciągu dnia może powodować pozbawienie snu w nocy. Śpij w ciągu dnia ściśle kontrolowany w ciągu godziny i spróbuj nie spać po trzeciej po południu.</string>
    '<String name = "sleep_time_on2" >• Wybierz czas ćwiczeń. Popołudniowe ćwiczenia pomagają przespać najlepszy czas, a regularne ćwiczenia fizyczne mogą poprawić jakość snu w nocy.</String>
    ' w:
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\fragment_hey_sleep.xml
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\fragment_sleep_day_history.xml
    ' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\smali_classes2\com\oudmon\band\ui\fragment\SleepDayFragment.smali

    ' algorytm liczenia jakosci snu:
    ' com/oudmon/algo/SleepAnalyzer.sleepAlgo(array(int),6):
    ' array:
    'v3(0) = sleep
    'v3(1) = deep
    'v3(2) = shallow
    'v3(3) = wakecount
    'v3(4) = age ' (date.now - birthday).totalmillis
    'v3(5) = gender
#End Region

#Region "Kroki"
    ' TodayStepData, bez Req wlasnego, cmd=0x48
    Private Async Function readKroki() As Task
        uiCurrentSteps.Text = "reading steps counter..."

        Dim aArr As Integer() = Await App.ReadKroki
        If aArr Is Nothing Then Exit Function

        Dim sCurrentSteps As String = "Total steps: " & aArr(1)
        Dim sRunningSteps As String = "Running steps: " & aArr(2)
        Dim sCalories As String = "Energy: " & (aArr(3) / 1000) & " kcal"
        Dim sDistance As String = "Distance: " & (aArr(4) / 1000) & " km"
        Dim sDuration As String = "Duration: " & App.Min2HrMin(aArr(5)) & " min"

        uiCurrentSteps.Text = sCurrentSteps
        uiRunningSteps.Text = sRunningSteps
        uiCalories.Text = sCalories
        uiDistance.Text = sDistance
        uiDuration.Text = sDuration

        App.TimerWriteSteps(aArr)

        ' obsluga w BandMainService$3
        ' 0,7,52, 0,0,0, 1,  103,90,0,3,246,1,74, crc=143
        ' wyswietlacz: 1844, = 0x734, = 7+52
        ' teoretycznie:
        ' 0,7,52 = kroki total
        ' 0,0,0 = running steps
        ' 1, 103, 90 = kalorie (wyswietlacz jest w kcal)
        ' 0,3,246 = walking distance (m)
        ' 1, 74 = sport duration, = 330
    End Function

#End Region

#Region "tetnoPlus"
    'ReadHeartRateReq	15	(long time).
    ' wywolywane z HeyRatePlusFragment.smali , oraz HeyRatePlusPresenter.smali
    ' smali_classes2\com\oudmon\band\ui\fragment\HeyRatePlusFragment.smali :
    ' obtainHeartRate(long time) =
    ' executeReqCmd( ReadHeartRateReq(long time), callback HeyRatePlusFragment$4)
    ' to z kolei z access$100(long time) ' calendar.timeinmillis / 1000 (timeinsecs)
    ' ale to wywolanie jest z HeyRatePlusFragment$3
    ' access$100(access$000 (=Calendar).getTimeInMillis / 1000)
    ' a kalendarz jest "dzisiaj", z wyzerowanymi godzina/minuta/sekunda (initCalendar)
    ' v2 = v0 = calendar, v1=14, v3=15; v2=cal.get(15) zoneoffset, v0(cal).set(millis,v2)
    Private Async Function readPulsePlus() As Task
        Dim dEpoch As Date = New Date(1970, 1, 1, 0, 0, 0)
        Dim iTimeSec As Double = (DateTime.Now.Subtract(dEpoch)).TotalSeconds
        iTimeSec = iTimeSec - (iTimeSec Mod 1)
        Dim c1 As Integer = iTimeSec Mod 256
        iTimeSec = iTimeSec \ 256
        Dim c2 As Integer = iTimeSec Mod 256
        iTimeSec = iTimeSec \ 256
        Dim c3 As Integer = iTimeSec Mod 256
        iTimeSec = iTimeSec \ 256
        Dim c4 As Integer = iTimeSec Mod 256   ' albo i bez modulo, bo juz powinno byc zero

        Dim aArr As Byte() = Await App.SendBandCommand(5, &H15, c4, c3, c2, c1)
        If aArr(0) <> &H15 Then Exit Function

        For i As Integer = 0 To aArr.GetUpperBound(0)
            Dim iPuls As Byte = aArr(i)
            Dim iTime As Long = ((((aArr(i + 1) * 256) + aArr(i + 2) * 256)) + aArr(i + 3) * 256) + aArr(i + 4)
            Dim dDate As Date = dEpoch
            dDate.AddSeconds(iTime)
        Next
        ' aArr(0) = (do 0xff)
        ' 4 bajty UTCtime
        ' 5 bajtowe, wiec moze 0xff / tetno, oraz time?

    End Function
#End Region

#Region "Dane zdrowotne"
    Private Async Function readZdrowko() As Task
        ' rsp/BpDataRsp w HealthHomeActivity.smali
        ' w sub readnextdata() jest execReqCmd(BpReadConformReq, HealthHomeActivity$10) - praktycznie empty?
        ' a wczesniej w obtainBlePressure jest execReqCmd(ReadPressureReq, HealthHomeActivity$2)
        ' obtainBle wywolywane w endSyncMonitorPressure z parametrem band/common/AppConfig.getSyncPressureTime
        ' a to jest zgodne z bandapi/entity/BlePressure;->time: (long time, int sbp, int dbp)
        ' access$200 (=onGetPressureSuccess), 300 (=dismissMyDialog), 400 (=notifyFragments), 500 (=showToast), 600 (=onGetPressureFailed)
        ' jest tu tez zestaw fields BLOOD_OXYGEN_TAG, BLOOD_PRESSURE_TAG, FATIGUE_TAG, HEART_RATE_TAG ..
        'ReadPressureReq		14	(long time)
        'BpReadConformReq	 e	0:ok,FF:err, 0...

        Dim dEpoch As Date = New Date(1970, 1, 1, 0, 0, 0)
        Dim dStart As Date = New Date(2018, 6, 1)   ' Date.Now
        Dim iTimeSec As Double = (dStart.Subtract(dEpoch)).TotalSeconds
        iTimeSec = iTimeSec - (iTimeSec Mod 1)
        Dim c1 As Integer = iTimeSec Mod 256
        iTimeSec = iTimeSec \ 256
        Dim c2 As Integer = iTimeSec Mod 256
        iTimeSec = iTimeSec \ 256
        Dim c3 As Integer = iTimeSec Mod 256
        iTimeSec = iTimeSec \ 256
        Dim c4 As Integer = iTimeSec Mod 256   ' albo i bez modulo, bo juz powinno byc zero

        Dim aArr As Byte() = Await App.SendBandCommand(5, &H14, c4, c3, c2, c1)
        If aArr(0) <> &H14 Then Exit Function

        For i As Integer = 0 To aArr.GetUpperBound(0)
            Dim iPuls As Byte = aArr(i)
            Dim iTime As Long = ((((aArr(i + 1) * 256) + aArr(i + 2) * 256)) + aArr(i + 3) * 256) + aArr(i + 4)
            Dim dDate As Date = dEpoch
            dDate.AddSeconds(iTime)
        Next
        ' aArr(0) = (do 0xff)
        ' 4 bajty UTCtime
        ' 5 bajtowe, wiec moze 0xff / tetno, oraz time?
    End Function
#End Region

    Private Async Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        Await readKroki()
        'Await readPulsePlus()   ' bywa unsupported
        'Await readZdrowko()     ' unsupported?
        'Await readSen()
        Await readKrokiHistorycznie()
    End Sub

    Private Sub uiSen_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiSen.Tapped
        App.ClipPut(uiSen.Text)
    End Sub

    Private Sub uiStartTetno_Tapped(sender As Object, e As TappedRoutedEventArgs)
        startMeasuringRTTetno()
    End Sub

#If False Then
    Private Sub uiStartTetno2_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiStartTetno2.Tapped
        startMeasuringTetno()
    End Sub

    Private Sub uiStartTlen_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiStartTlen.Tapped
        startMeasuringTlen()
    End Sub

    Private Sub uiStartFatyga_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiStartFatyga.Tapped
        startMeasuringFatyga()
    End Sub

    Private Sub uiStartPressure_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiStartPressure.Tapped
        startMeasuringPressure()
    End Sub

    Private Sub uiStartHealth_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiStartHealth.Tapped
        startMeasuringHealth()
    End Sub
#End If
End Class

'     <string name="height">wysokość</string>
'    <string name="birthday">data urodzenia</string>
'    <string name="weight">waga</string>
'   <string name="woman">Kobieta</string>
' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\activity_init_info.xml
' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\activity_set_info.xml
' E:\NoweDVD\Public\Programs\dex2jar\viaPNIE.1.30.07\res\layout\activity_userinfo.xml

