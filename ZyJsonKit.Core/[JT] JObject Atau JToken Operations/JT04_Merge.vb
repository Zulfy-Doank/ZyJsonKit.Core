Imports Newtonsoft.Json.Linq

''' <summary>
''' JO04_JSON Merge
''' Menggabungkan dua atau lebih JObject
''' Support merge strategy (union, replace, concat)
''' [PF03] Exception-safe
''' [PF04] Null-safe
''' </summary>
Public NotInheritable Class JT04_Merge

    ''' <summary>
    ''' Merge dua JObject (target akan dimodifikasi)
    ''' </summary>
    ''' <param name="target">JObject target (akan diupdate)</param>
    ''' <param name="sumber">JObject sumber</param>
    ''' <param name="timpaNilai">True untuk timpa value yang sudah ada</param>
    ''' <returns>JObject hasil merge</returns>
    Public Shared Function Gabungkan(
        target As JObject,
        sumber As JObject,
        Optional timpaNilai As Boolean = True) As JObject

        ' Handle null
        If target Is Nothing AndAlso sumber Is Nothing Then Return New JObject()
        If target Is Nothing Then Return sumber
        If sumber Is Nothing Then Return target

        Try
            Dim mergeSetting = If(timpaNilai,
                MergeArrayHandling.Union,
                MergeArrayHandling.Concat)

            target.Merge(sumber, New JsonMergeSettings With {
                .MergeArrayHandling = mergeSetting,
                .MergeNullValueHandling = MergeNullValueHandling.Ignore
            })

            Return target
        Catch
            Return target
        End Try
    End Function

    ''' <summary>
    ''' Merge tanpa memodifikasi original (return object baru)
    ''' </summary>
    Public Shared Function GabungkanSalin(
        target As JObject,
        sumber As JObject,
        Optional timpaNilai As Boolean = True) As JObject

        ' ✅ IDE0059 Fix: Hapus assignment awal = Nothing
        ' Inisialisasi langsung dengan nilai yang benar via ternary
        ' Eliminasi assignment sia-sia yang langsung di-overwrite
        Dim clone As JObject = If(
            target IsNot Nothing,
            DirectCast(target.DeepClone(), JObject),
            New JObject())

        If sumber IsNot Nothing Then
            clone = Gabungkan(clone, sumber, timpaNilai)
        End If

        Return clone
    End Function

    ''' <summary>
    ''' Merge banyak JObject sekaligus
    ''' </summary>
    ''' <param name="daftarObject">Daftar JObject yang akan digabung</param>
    ''' <returns>JObject hasil merge</returns>
    Public Shared Function GabungkanBanyak(
        ParamArray daftarObject As JObject()) As JObject

        If daftarObject Is Nothing OrElse daftarObject.Length = 0 Then
            Return New JObject()
        End If

        Dim hasil As New JObject()

        For Each obj In daftarObject
            If obj IsNot Nothing Then
                hasil = Gabungkan(hasil, obj, True)
            End If
        Next

        Return hasil
    End Function

    ''' <summary>
    ''' Merge dengan strategy: timpa hanya jika target null/kosong
    ''' </summary>
    Public Shared Function GabungkanIsiKosong(
        target As JObject,
        sumber As JObject) As JObject

        If target Is Nothing Then Return If(sumber, New JObject())
        If sumber Is Nothing Then Return target

        Try
            For Each prop In sumber.Properties()
                Dim existingProp = target.Property(prop.Name)

                If existingProp Is Nothing Then
                    ' Properti belum ada, tambahkan
                    target.Add(prop.Name, prop.Value.DeepClone())
                ElseIf existingProp.Value.Type = JTokenType.Null OrElse
                       (existingProp.Value.Type = JTokenType.String AndAlso
                        String.IsNullOrEmpty(existingProp.Value.ToString())) Then
                    ' Properti ada tapi null/kosong, isi dengan nilai baru
                    existingProp.Value.Replace(prop.Value.DeepClone())
                End If
                ' Jika properti sudah ada dan berisi, skip
            Next

            Return target
        Catch
            Return target
        End Try
    End Function

    ''' <summary>
    ''' Merge dengan konflik resolution callback
    ''' </summary>
    ''' <param name="target">JObject target</param>
    ''' <param name="sumber">JObject sumber</param>
    ''' <param name="resolusiKonflik">
    ''' Callback untuk menyelesaikan konflik (targetValue, sourceValue) => resolvedValue
    ''' </param>
    ''' <returns>JObject hasil merge</returns>
    Public Shared Function GabungkanDenganResolusi(
        target As JObject,
        sumber As JObject,
        resolusiKonflik As Func(Of String, JToken, JToken, JToken)) As JObject

        If target Is Nothing Then Return If(sumber, New JObject())
        If sumber Is Nothing Then Return target

        Try
            For Each prop In sumber.Properties()
                Dim existingProp = target.Property(prop.Name)

                If existingProp Is Nothing Then
                    ' Tidak ada konflik, tambahkan langsung
                    target.Add(prop.Name, prop.Value.DeepClone())
                ElseIf resolusiKonflik IsNot Nothing Then
                    ' Ada konflik, gunakan callback resolusi
                    Dim resolvedValue = resolusiKonflik(prop.Name, existingProp.Value, prop.Value)
                    existingProp.Value.Replace(If(resolvedValue, prop.Value.DeepClone()))
                Else
                    ' Timpa dengan nilai baru (default)
                    existingProp.Value.Replace(prop.Value.DeepClone())
                End If
            Next

            Return target
        Catch
            Return target
        End Try
    End Function

End Class