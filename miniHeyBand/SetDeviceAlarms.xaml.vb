' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SetDeviceAlarms
    Inherits Page

    ' ustawianie roznych alarmow
    ' ReadAlarmReq		24	(int alarmIndex)
    ' ReadDrinkAlarmReq	28	(int alarmIndex)
    ' SetAlarmReq		23
    ' SetSitLongReq		25
    ' SetDrinkAlarmReq	27

    '  CMD_GET_SIT_LONG As Byte = 0x26t		' jest samodzilene obtainSedentarySettings

End Class
