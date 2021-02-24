
var ChildSelectName = "";
var ChildFieldValue = "";
var countrySelect = "";

SP.SOD.executeFunc('sp.js', 'SP.ClientContext', IntialPage);

function IntialPage() {
    $(document).ready(function () {
        SPCustomServiceGetSelect("Region", "Country", "RegionCountryList", "Region", "Country");//源父字段名，源子字段名，源列表列表，父字段名，子字段名    
    })
}

function SPCustomServiceGetSelect(SourceParentField, SourceChildField, SourceList, ParentField, ChildField) {
    ChildSelectName = SourceChildField;
    ChildFieldValue = ChildField;

    countrySelect = $("select[title^=" + ChildSelectName + "]").find("option:selected").text();

    $("select[title=" + ChildSelectName + "]").html(countrySelect);
    var Selectltem = $("select[title^='" + SourceParentField + "']").change(function () {
        var SelectItemValue = $("select[title^=" + SourceParentField + "]").find("option:selected").text();
        SPCustomServiceGetItems(SelectItemValue, SourceList, ParentField, ChildField);
    });

    var cuSelectItemValue = $("select[title^=" + SourceParentField + "]").find("option:selected").text();
    SPCustomServiceGetItems(cuSelectItemValue, SourceList, ParentField, ChildField);
}

function SPCustomServiceGetItems(SelectItemValue, SourceList, ParentField, ChildField) {
    var Context = new SP.ClientContext.get_current();
    var Web = Context.get_web();
    var List = Web.get_lists().getByTitle(SourceList);
    var Query = new SP.CamlQuery();
    Query.set_viewXml('<View><Query><Where><Eq><FieldRef Name=\'' + ParentField + '\' /><Value Type=\'Lookup\'>' + SelectItemValue + ' </Value></Eq></Where></Query></View>');
    Allltems = List.getItems(Query);
    Context.load(Allltems, 'Include(Id,' + ChildField + ')');
    Context.executeQueryAsync(Function.createDelegate(this, this.Successed), Function.createDelegate(this, this.Failed));
}

function Successed() {
    var SelectText = "<option value='0'></option>";
    var ListEnumerator = this.Allltems.getEnumerator();
    while (ListEnumerator.moveNext()) {
        var Currentltem = ListEnumerator.get_current();
        if (countrySelect == Currentltem.get_item(ChildFieldValue)) {
            SelectText += "<option selected = 'selected' value='" + Currentltem.get_id() + "' >" + Currentltem.get_item(ChildFieldValue) + "</option>";
        } else {
            SelectText += "<option value='" + Currentltem.get_id() + "' >" + Currentltem.get_item(ChildFieldValue) + "</option>";
        }
    }

    $("select[title=" + ChildSelectName + "]").html(SelectText);
}

function Failed() {
}
