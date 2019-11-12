<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MainPage.aspx.cs" Inherits="MainPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link rel="stylesheet" href="StyleSheet.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
	<script type="text/javascript">
        function ReceiveServerData(params) {
            var param = params.split('`');
            switch (param[0]) {
                case "insertStudent":
                    debugger;
                    document.getElementsByClassName('newStudent')[0].outerHTML = param[1];
                    break;
                case "insertTest":
                    document.getElementsByClassName('newTestRow')[0].outerHTML = param[1];
                    $('.subjectTd').hide();
                    break;
                case "selectStudentTests":
                    var studentDetail = document.getElementsByClassName("studentDetail")[0]
                    studentDetail.innerHTML = param[1];
                    studentDetail.style.display = '';
                    break;
                case "deleteStudent":
                    document.getElementById("id_" + param[1]).remove();
                    break;
                case "deleteTest":
                    document.getElementById("testId_" + param[1]).remove();
                    break;                    
                case "updateStudent":
                    debugger;
                    document.getElementById("id_" + param[1]).classList.toggle('viewMode');
                    var phoneVal = document.getElementById("id_" + param[1]).getElementsByClassName('phone')[0].value;
                    if (phoneVal != null && phoneVal != "") {
                        phoneVal = phoneVal
                            .match(/\d*/g).join('')
                            .match(/(\d{0,3})(\d{0,3})(\d{0,4})/).slice(1)
                            .map((a, i) => (a + ['-', '-', ''][i])).join('');
                        document.getElementById("id_" + param[1]).getElementsByClassName('phone')[0].value = phoneVal;
                    }
                    $(".input").prop('disabled', true);     
                    
                    break;
                case "updateTest":
                    var gradeTd = document.getElementsByClassName("grade_" + param[1])[0];
                        gradeTd.innerHTML = gradeTd.getElementsByTagName('input')[0].value;
                    var dateTd = document.getElementsByClassName("date_" + param[1])[0];
                    var dateTdHolder = dateTd.getElementsByTagName('input')[0].value;
                    var dateTdTemp = new Date(dateTdHolder + " 12:00:00");
                         dateTd.innerHTML = getFormattedDate(dateTdTemp);
                    document.getElementById("testId_" + param[1]).classList.toggle('viewMode');
                    break;
            }
        }
        function closeStudentDetail() {
            document.getElementsByClassName("studentDetail")[0].style.display = 'none';
        }
        function validateEmail(mail) {
            if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
                return (true)
            }
            return (false)
        }
        function validateDate(d) {
            var date_regex = /^(0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])[\/\-]\d{4}$/;
            if (!(date_regex.test(d))) {
                return false;
            }
            else { return true; }
        }
        function validateRange(r) {
            var reg = /^[0-9]$|^[1-9][0-9]$|^(100)$/;
            if (reg.test(r)) { return (true) }
            return (false)
        }
        function validatePhone(p) {
            var phoneRe = /^[2-9]\d{2}[2-9]\d{2}\d{4}$/;        
            if (phoneRe.test(p)) { return (true) }
            return (false)
        }
        
        function deleteStudent(id) {
          if(confirm("Are you sure you want to delete?")==true)
            CallServer("deleteStudent`" + id);
        }
        function deleteTest(id) {
            CallServer("deleteTest`" + id);
        }
		function addStudent() {
			var table = document.getElementsByClassName("studentsList")[0].firstChild;
            var html = "<tr class='newStudent'><td><input placeholder='Last Name*'/></td><td><input placeholder='First Name*'/></td><td><input placeholder='Email*'/></td><td><input placeholder='Phone'/></td><td><img src='Images/save.svg' onclick='insertStudent()' /></td><td><img src='Images/cancel.svg' onclick='clearStudent()' /></td><td></td></tr>";
            table.insertAdjacentHTML("beforeend", html);
        }       
        function testView(id) {
            CallServer("selectStudentTests`" + id + '`' + event.target.className.split("_")[1]);
        }
        function insertTest(id) {
            var data = document.getElementsByClassName('newTestRow')[0].getElementsByTagName('input');
            var sub = document.getElementsByClassName('selectedSubject')[0].innerText;
            getTestValues(data, id,-1,sub, 'insertTest');
        }
        function getTestValues(data, studentid, testid,sub, insertCall) {
            var msg = "";
            var grade = data[0].value;
            var dateTemp =new Date(data[1].value);
            var date = getFormattedDate(dateTemp);
            var  isGradeValid = validateRange(grade);
           var  isDateValid = validateDate(date);
            if (!isGradeValid) msg += "Please enter a grade 0-100.\n";
            if (!isDateValid) msg += "Please enter a valid date.\n";            
            msg == "" ? CallServer(insertCall+"`" + studentid + "`" + sub + "`" + grade + "`" + date + "`"+testid) : alert(msg);
        }
        function insertStudent() {
            var data = document.getElementsByClassName('newStudent')[0].getElementsByTagName('input');
            getStudentValues(data, -1,"insertStudent");
        }
        function getStudentValues(data,studentid,callparam) {
            var msg = "";
            var fn = data[1].value;
            if (fn == null || fn == "") msg += "Please insert a valid first name.\n";
            var ln = data[0].value;
            if (ln == null || ln == "") msg += "Please insert a valid last name.\n";
            var  email = data[2].value;
            var phone = data[3].value.replace(/\D/g, "");          
            var isEmailValid = validateEmail(email);
            if (!isEmailValid) msg += "Please enter a valid email.\n"
            var  isPhoneValid = phone != null && phone != "" ? validatePhone(phone) : true;
            if (!isPhoneValid) msg += "Please enter a valid phone number."
            msg == "" ?CallServer(callparam+"`" + fn + "`" + ln + "`" + email + "`" + phone + "`"+studentid) : alert(msg);
        }
        function clearStudent() {
            document.getElementsByClassName('newStudent')[0].remove();
        }
        function clearTest() {
            document.getElementsByClassName('newTestRow')[0].remove();
        }
        function editStudent(id) {
            event.target.parentElement.parentElement.classList.toggle('viewMode');
            var inp = event.target.parentElement.parentElement.getElementsByTagName('input');
            $(inp).prop('disabled', false);
        }
        function editTest(id) {
            var gradeTd = document.getElementsByClassName("grade_" + id)[0]
            gradeTd.innerHTML = "<input value=" + gradeTd.innerText + " />";// max='100' min='0'
            var dateTd = document.getElementsByClassName("date_" + id)[0];
            var dateString = new Date(dateTd.innerText).toISOString().substring(0, 10);
            dateTd.innerHTML = "<input  type='date' value=" + dateString + " />";
            document.getElementById("testId_" + id).classList.toggle('viewMode');
        }
        function getFormattedDate(date) {
            var year = date.getFullYear();
            var month = (1 + date.getMonth()).toString();
            month = month.length > 1 ? month : '0' + month;
            var day = date.getDate().toString();
            day = day.length > 1 ? day : '0' + day;
            return month + '/' + day + '/' + year;
        }
        function undoTest(id) {
            var gradeTd = document.getElementsByClassName("grade_" + id)[0];
            gradeTd.innerHTML = gradeTd.getElementsByTagName('input')[0].defaultValue;
            var dateTd = document.getElementsByClassName("date_" + id)[0];
            var dateTdHolder=dateTd.getElementsByTagName('input')[0].defaultValue;
            var dateTdTemp = new Date(dateTdHolder + " 12:00:00");      
            dateTd.innerHTML =getFormattedDate(dateTdTemp);
            document.getElementById("testId_" + id).classList.toggle('viewMode');
        }
        function undoStudent(s) {
            var data = event.target.parentNode.parentNode.getElementsByTagName('input');//document.getElementById('id_' + id).getElementsByTagName('input');
            for (var i = 0; i < data.length; data++) {
                data[i].value = data[i].defaultValue;
            }            
            event.target.parentElement.parentElement.classList.add('viewMode');
            $("input").prop('disabled', true);
        }
        function updateStudent(id) {
            var data = document.getElementById('id_'+id).getElementsByTagName('input');
            getStudentValues(data, id, "updateStudent");
        }
        function updateTest(testid) {
            var data = document.getElementById('testId_' + testid).getElementsByTagName('input');
            var studentid = document.getElementsByClassName('subjectDetailTbl')[0].id.split("_")[1];
            var sub = document.getElementById('testId_' + testid).classList[0].split("_")[1];          
            getTestValues(data, studentid, testid,sub, "updateTest");
        }
        function addTest(id) {
            var sub = document.getElementsByClassName('selectedSubject')[0].innerText;
            var table = document.getElementsByClassName("subjectDetailTbl")[0].firstChild;
            var html = "<tr class='newTestRow sub_" + sub.toLowerCase() + "'><td><input max='100' min='0' placeholder='Grade'/></td><td><input placeholder='Date' type='date'/></td><td style='border-right:none'><img src='Images/save.svg' onclick='insertTest(" + id +")' /></td><td style='border-left:none'><img src='Images/cancel.svg' onclick='clearTest()' /></td></tr>";
            table.insertAdjacentHTML("beforeend", html);
        }
        function filterSub() {
            var inps = document.getElementsByClassName("subjectDetailTbl")[0].getElementsByTagName("input");
            var count = inps.length;
            if (count > 0) {
                var inp1 = inps[0];
                var inp2 = inps[1];
                inp1.parentElement.innerHTML = inp1.defaultValue;
                var dateTdTemp = new Date(inp2.defaultValue + " 12:00:00");
                inp2.parentElement.innerHTML =   getFormattedDate(dateTdTemp);
            }
            //clear anything in edit or add mode
            $(".subjectDetailTbl tr").addClass("viewMode");
            $('.newTestRow').remove();
            //select new subject
            var sub = event.target.innerText;
            document.getElementsByClassName('selectedSubject')[0].classList.remove('selectedSubject');
            event.target.classList += "selectedSubject";
            //hide rows and items not applicable for that catagory
            var trs=    document.getElementsByClassName("subjectDetailTbl")[0].getElementsByTagName("tr");
                for (var i = 1; i < trs.length; i++) {
                    (trs[i].classList[0] != "sub_" + sub) && sub != 'All' ? trs[i].style.display = 'none' : trs[i].style.display = '';
            }
            if (document.getElementsByClassName('selectedSubject')[0].innerText == 'All') {             
                $('.subjectTd').show();
                document.getElementsByClassName("newTest")[0].style.display = 'none';
            }
            else {
                $('.subjectTd').hide();
                document.getElementsByClassName("newTest")[0].style.display = 'block';
            }      
        }
	</script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="headerBkgrd">
            <div class="headerTxt">Greenwood Academy</div>
         </div>
        <div class="headerSub">Student Contact List</div>	
        <div id="studentList" runat="server" style="width:1000px;margin:auto"></div>
      <div class="studentDetail" style="display:none;"></div>
    </form>
</body>
</html>
