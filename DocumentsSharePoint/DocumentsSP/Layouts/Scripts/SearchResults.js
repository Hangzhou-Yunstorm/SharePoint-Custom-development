window.onload = function () {

}

// 搜索
function search() {
    var querytext = $("#Key").val();
    var start = $("#Start").val();
    var rowLimit = $("#Limit").val();
    $.getJSON(webUrl + "/_api/search/query?querytext='" + querytext + "'&sortlist='write:descending'&clienttype='ContentSearchRegular'&StartRow=" + start + "&RowLimit=" + rowLimit,
        function (data) {
            var rows = data.PrimaryQueryResult.RelevantResults.Table.Rows;

            $("#resultTable").empty();
            var row = "<tr><td>Title</td><td>Path</td></tr>";
            $('#resultTable').append(row);
            for (var m = 0; m < rows.length; m++) {
                var cells = rows[m].Cells;
                var title, path;
                for (var n = 0; n < cells.length; n++) {
                    var prop = cells[n]
                    if (prop.Key == "Title")
                        title = prop.Value;
                    if (prop.Key == "Path")
                        path = prop.Value;
                }
                row = "<tr><td>" + title + "</td><td>" + path + "</td></tr>";
                $('#resultTable').append(row);
            }
        })
}

// Post
function StartSearch() {
    // 在搜索之前先要取到X-RequestDigest, 并使用Ajax request的Header 传到服务器端，如果没有X-RequestDigest 会出现 403 错误  
    $.ajax(
    {
        url: webUrl + "/_api/contextinfo",
        type: "Post",
        dataType: "xml",
        contentType: "text/xml; charset=\"utf-8\"",
        complete: ProcessDigest
    });
}

function ProcessDigest(xData, status) {
    var xRequestDigest = xData.responseText.SubStringBetween("<d:FormDigestValue>", "</d:FormDigestValue>");

    var querytext = $("#Key").val();
    var start = $("#Start").val();
    var rowLimit = $("#Limit").val();

    $.ajax({
        url: webUrl + "/_api/search/postquery",
        type: "Post",
        dataType: "application/json;odata=verbose",
        data: JSON.stringify({
            'request': {
                'Querytext': querytext,
                'StartRow': start,
                'RowLimit': rowLimit,
                'SelectProperties': {
                    'results': ['Title', 'ContentSource', 'DisplayAuthor', 'Path', 'Write']
                },
                'SortList': { 'results': [{ 'Property': 'write', 'Direction': '1' }] },
            }
        }),
        headers: {
            "accept": "application/json;odata=verbose",
            "content-type": "application/json;odata=verbose",
            "X-RequestDigest": xRequestDigest
        },
        complete: ProcessSearchResult
    });
}

// 处理Search Rest API返回的数据，将其转换成 Josn对象并显示在表格中  
function ProcessSearchResult(xData, status) {
    if (xData.status == 200) {
        // 将搜索结果转换成 Josn对象  
        var josnData = $.parseJSON(xData.responseText);
        // 清空表格内容   
        $("#resultTable").empty();
        var row = "<tr><td>Title</td><td>ContentSource</td><td>DisplayAuthor</td><td>Path</td></tr>";
        $('#resultTable').append(row);
        // 遍历搜索结果并逐条插入表格   
        $.each(josnData.d.postquery.PrimaryQueryResult.RelevantResults.Table.Rows.results, function () {
            var title;
            var contentSource;
            var displayAuthor;
            var path
            $.each(this.Cells.results, function () {
                if (this.Key == "Title")
                    title = this.Value;

                if (this.Key == "ContentSource")
                    contentSource = this.Value;

                if (this.Key == "DisplayAuthor")
                    displayAuthor = this.Value;

                if (this.Key == "Path")
                    path = this.Value;
            });
            row = '<tr><td>' + title + '</td><td>' + contentSource + '</td><td>' + displayAuthor + '</td><td>' + path + '</td></tr>';
            $('#resultTable').append(row);
        });
    }
}

// String 方法扩展， 由于$.ParseXML方法报错unsupported pseudo,所以采用分割字符串的笨方法来取FormDigestValue的值  
String.prototype.SubStringBetween = function (prefix, suffix) {
    var strArray = this.split(prefix);
    var strArray1 = strArray[1].toString().split(suffix);
    return strArray1[0];
};