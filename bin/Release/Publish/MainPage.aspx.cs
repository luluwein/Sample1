using System;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public partial class MainPage : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler
{
	protected void Page_Load(object sender, EventArgs e)
	{
           if (!IsPostBack)
			{
                String cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "context");
				String callbackScript = "function CallServer(arg, context)" + "{ " + cbReference + "} ;";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CallServer", callbackScript, true);
          
                studentList.InnerHtml = buildStudentListHTML();
                HttpContext.Current.Session["RESULT"] = "";
        }
    }
    
    public string buildTestTr(Test t)
    {
        string dateString = t.TestDate.ToString();
        string dateParse = DateTime.Parse(dateString).ToString("MM/dd/yyyy");
        string html = "<tr id='testId_" +t.TestID + "' class='sub_" +t.Subject + " viewMode'><td class='subjectTd'>" + t.Subject + "</td><td class='grade_" + t.TestID + "'>" + t.Grade + "</td><td class='date_" +t.TestID + "'>" + dateParse + "</td><td style='border-right:none'><img onclick='deleteTest(" + t.TestID + ")'src='Images/trash.svg' /><img onclick='updateTest(" + t.TestID + ")' src='Images/save.svg' /></td><td style='border-left:none'><img onclick='editTest(" + t.TestID + ")'src='Images/editTest.svg' /><img onclick='undoTest(" + t.TestID + ")' src='Images/cancel.svg' /></td></tr>";
        return html;
    }
    public string buildStudentTr(Student s)
    {
        string phone = s.Phone;
        if (s.Phone!="" & s.Phone != null) phone = Regex.Replace(s.Phone, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");
        string html = "<tr class='viewMode' id='id_" + s.StudentID + "'><td><input disabled value=" + s.LastName + " /></td><td><input disabled value=" + s.FirstName + " /></td><td><input  disabled value=" + s.Email + " /></td><td><input class='phone' disabled value=" + phone+ " ></input></td><td><img onclick='deleteStudent(" + s.StudentID + ")'src='Images/deleteUser.svg' /><img onclick='updateStudent(" + s.StudentID + ")' src='Images/save.svg' /></td><td ><img onclick='editStudent()'src='Images/editUser.svg' /><img onclick='undoStudent()' src='Images/cancel.svg' /></td><td ><div onclick='testView(" + s.StudentID + ")' class='detailBtn sName_" + s.FirstName + " " + s.LastName + "'>Details</div></td></tr>";
        return html;
    }
  
    public void RaiseCallbackEvent(String eventArgument)
	{
		string[] param = eventArgument.Split('`');
		switch (param[0])
		{
            case "insertTest":
                string iDate = param[4].ToString();
                DateTime oDate = DateTime.Parse(iDate);
                Test t = new Test(Convert.ToInt32(param[1]), param[2],Convert.ToInt32( param[3]), oDate);
                t.TestID= t.InsertTest(Convert.ToInt32(param[1]), param[2], param[3],oDate);
                string htmlt = buildTestTr(t);// "<tr id='testid_"+t.TestID+"' class=sub_" + param[2].ToString() + "><td>" +param[3].ToString() + "</td><td>" +param[4] + "</td><td>" + param[2].ToString() + "</td></tr>";
                Session["RESULT"] = "insertTest" + "`" + htmlt;
                break;

            case "updateTest":
                string iDate2 = param[4].ToString();
                DateTime oDate2 = DateTime.Parse(iDate2);
                Test tu = new Test(Convert.ToInt32(param[1]), param[2], Convert.ToInt32(param[3]), oDate2);
                tu.TestID =Convert.ToInt32(param[5]);
                tu.UpdateTest(tu.TestID,tu.Grade,tu.TestDate);
                Session["RESULT"] = "updateTest`"+ tu.TestID;
                break;

            case "deleteTest":
                Test test1 = new Test();
                test1.DeleteTest(Convert.ToInt32(param[1]));
                Session["RESULT"] = "deleteTest" + "`" + param[1];
                break;

            case "selectStudentTests":
                Student st = new Student();
                st.StudentID = Convert.ToInt32(param[1]);
                Session["RESULT"] = "selectStudentTests" + "`" +  buildStudentTestHTML(st, param[2].ToString());
                break;

            case "insertStudent":
                Student s = new Student(param[1], param[2], param[3], param[4]);
                s.StudentID = s.InsertStudent(s.FirstName, s.LastName, s.Email, s.Phone);
                string html = buildStudentTr(s);
                Session["RESULT"] = "insertStudent" + "`" + html;
                break;

             case "updateStudent":
                Student sUpdate = new Student(param[1], param[2], param[3], param[4]);
                sUpdate.StudentID =Convert.ToInt32( param[5]);
                sUpdate.UpdateStudent(sUpdate.StudentID, sUpdate.FirstName, sUpdate.LastName, sUpdate.Email, sUpdate.Phone);
                        Session["RESULT"] = "updateStudent" + "`"+ sUpdate.StudentID;
                break;

              case "deleteStudent":
                Student s1 = new Student();
                s1.DeleteStudent(Convert.ToInt32(param[1]));              
                Session["RESULT"] = "deleteStudent" + "`" + param[1];
                break;
        }
        
	}
    public string GetCallbackResult()
    {
        string res = Session["RESULT"].ToString();
        return res;
    }

    /**********************/
    //TODO prevent coloumn bouncing
    //TODO rearrange layouts, redo icons,colors, create header tabs ontop of page to navigate


    //TODO create stored procedures and generate webservices instead of calling from class page
    //TODO create user login and get role
    public string LoginAndGetRole(string username, string password)
	{
		using (SqlConnection db = new SqlConnection(@"Data Source=(local); Initial Catalog=NCS; Integrated Security=false;userid=" + username + ";password=" + password))
		{
			using (SqlCommand getRole = new SqlCommand("findUserRole", db))
			{
				db.Open(); //crashes at this line
				getRole.CommandType = CommandType.StoredProcedure;
				getRole.Parameters.Add(new SqlParameter("@Username", username));
				SqlDataReader reader = getRole.ExecuteReader();
			}
			db.Close();
			return "";
		}
	}

    //TODO create layout where every single student is listed based on subject and access test scores there
    public string buildTestHTML()
    {
        string html = "<table class='studentsList'>";
        html += "<tr class='title'><td></td><td></td><td>Last Name</td><td>First Name</td><td>Subject</td><td>Grade</td><td>Date</td> </tr>";
        DataSet ds = GetTestDS();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Test t = new Test(dr);
            t.TestID = Convert.ToInt32(dr["TestID"]);
            html += buildTestTr(t);
        }
        html += "</table>";
        return html;
    }
    //TODO
    public DataSet GetTestDS()
    {
        string connectionString = @"Data Source=(local);Initial Catalog=NCS;Persist Security Info=True;Integrated Security=true;";// User ID=sa;Password=demol23";
        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter data = new SqlDataAdapter();
        SqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT  tests.StudentID, students.FirstName,students.LastName,tests.Grade, tests.Date,tests.Subject,tests.TestID FROM   TESTS INNER JOIN STUDENTS ON  TESTS.StudentID = STUDENTS.StudentID ORDER BY  Tests.Grade ASC";
        data.SelectCommand = cmd;
        DataSet ds = new DataSet();
        conn.Open();
        data.Fill(ds);
        conn.Close();
        return ds;
    }
    //TODO make subject list data driven with capability of adding more subjects
    public string buildStudentTestHTML(Student st, string name)
    {
        DataSet ds = st.GetTestDS(st.StudentID);
        string html = "<div class='closePP' onclick='closeStudentDetail()'>x</div>" +
            "<table style='height: 100%;width:100%'><tbody><tr><td colspan='2'>" +
            "<div style='width:100%;text-align: center;font-size: 40px;color: #4490a9;text-indent: 80px;'>" + name + "</div>" +
            "</td></tr><tr><td style=' height: 100%;width:150px;'><div style='width: 150px;height: 100%;background: #add8e614;margin-right:-33px;'>" +
            "<table style='    height: 250px;    width: 100%; text-align: center;' class='subjectList'><tbody>" +
            "<tr><td class='selectedSubject' onclick='filterSub()'>All</td></tr><tr><td  onclick='filterSub()'>Math</td></tr><tr><td onclick='filterSub()'>English</td></tr><tr><td onclick='filterSub()'>Science</td></tr><tr><td onclick='filterSub()'>History</td></tr></tbody></table></div></td>" +
            "<td style=' vertical-align: baseline;'><div style='height: calc(100% - 20px); overflow: auto;margin-left:-5px; '>" +
            "<table class='subjectDetailTbl' id='testTbl_" + st.StudentID + "'><tbody><tr style=' font-weight: bold;'><td class='subjectTd'>Subject</td><td>Grade</td><td>Date</td><td colspan='2' style='width:46px'><img class='newTest'  onclick='addTest(" + st.StudentID + ")' src='Images/test.svg' style='height:25px;display:none' /></td></tr>";
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Test t = new Test(dr);
            t.TestID = Convert.ToInt32(dr["TestID"]);
            html += buildTestTr(t);
        }
        html += "</tbody></table></div></td></tr></tbody></table>";
        return html;
    }

    public string buildStudentListHTML()
    {
        string html = "<table class='studentsList'>";
        html += "<tr class='title'><td>Last Name</td><td>First Name</td><td>Email</td><td>Phone</td><td colspan='2'></td><td><img onclick='addStudent()' src='Images/addUser.svg' style='height: 25px;display:block;margin:auto;'/></td></tr>";
        Student stu = new Student();
        DataSet ds = stu.GetStudentsDS();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Student s = new Student(dr);
            html += buildStudentTr(s);
        }
        html += "</table>";
        return html;
    }

}
