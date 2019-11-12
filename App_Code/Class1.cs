using System;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// student and test classes
/// </summary>

public class Test
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Grade { get; set; }
    public string Subject { get; set; }
    public int StudentID { get; set; }
    public int TestID { get; set; }
    public DateTime TestDate { get; set; }
    string connectionString = @"Data Source=(local);Initial Catalog=NCS;Persist Security Info=True;Integrated Security=true;";// User ID=sa;Password=demol23";
    public Test()
    {
    }
    public Test(DataRow dr)
    {
        FirstName = dr["FirstName"].ToString();
        LastName = dr["LastName"].ToString();
        Grade = Convert.ToInt32( dr["Grade"]);
        Subject = dr["Subject"].ToString();
        TestDate =DateTime.Parse(dr["Date"].ToString());
        StudentID = Convert.ToInt32(dr["StudentID"]);
    }
    public Test(int studentid,string subject, int grade, DateTime date)
    {
        StudentID = studentid;
        Subject = subject;
        Grade = grade;
        TestDate = date;
    }
    public int InsertTest(int studentid, string subject, string grade, DateTime datetest)
    {
        int result=-1;
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                String query = "INSERT INTO dbo.Tests ( StudentID, Subject, Grade, Date)OUTPUT Inserted.TestID VALUES ( @StudentID, @Subject, @Grade, @Date) ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentid);
                    command.Parameters.AddWithValue("@Subject", subject);
                    command.Parameters.AddWithValue("@Grade", grade);
                    command.Parameters.AddWithValue("@Date", datetest);
                    connection.Open();
                    result = (int)command.ExecuteScalar();
                    connection.Close();
                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error inserting data into Database!");                    
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Bad character:" + ex.ToString());
        }
        return result;
       
    }
    public void UpdateTest(int testid,  int grade,  DateTime testdate)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                String query = "UPDATE dbo.Tests SET Grade=@Grade, Date=@Date  WHERE  TestID = @TestID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Grade", grade);
                    command.Parameters.AddWithValue("@Date", testdate);
                    command.Parameters.AddWithValue("@TestID", testid);
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    connection.Close();
                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error updating data into Database!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Bad character:" + ex.ToString());
        }
    }
    public void DeleteTest(int testid)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                String query = "Delete From Tests Where TestID = @TestID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@TestID", SqlDbType.Int).Value = testid;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();//
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Bad character:" + ex.ToString());
        }
    }
}

    public class Student
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public int StudentID { get; set; }
		string connectionString = @"Data Source=(local);Initial Catalog=NCS;Persist Security Info=True;Integrated Security=true;";// User ID=sa;Password=demol23";

		public Student()
		{
		}
		public Student(DataRow dr)
		{
			FirstName = dr["FirstName"].ToString();
			LastName = dr["LastName"].ToString();
			Email = dr["Email"].ToString();
			Phone =dr["Phone"].ToString();
			StudentID = Convert.ToInt32(dr["StudentID"]);
		}
        public Student( string firstname, string lastname, string email, string phone )
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            Phone = phone;        
        }
        public DataSet GetTestDS(int studentid )
        {
            DataSet ds = new DataSet();
            try
            {
                string connectionString = @"Data Source=(local);Initial Catalog=NCS;Persist Security Info=True;Integrated Security=true;";// User ID=sa;Password=demol23";
                SqlConnection conn = new SqlConnection(connectionString);
                SqlDataAdapter data = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT  tests.StudentID, students.FirstName,students.LastName,tests.Grade, tests.Date,tests.Subject,tests.TestID FROM   TESTS INNER JOIN STUDENTS ON  TESTS.StudentID = STUDENTS.StudentID WHERE TESTS.StudentID=@StudentID  ORDER BY  Tests.Date ASC";
                cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentid;
                data.SelectCommand = cmd;           
                conn.Open();
                data.Fill(ds);
                conn.Close();            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bad character:" + ex.ToString());
            }
            return ds;
        }
        public DataSet GetStudentsDS()
        {
            DataSet ds = new DataSet();
            try
            {
                string connectionString = @"Data Source=(local);Initial Catalog=NCS;Persist Security Info=True;Integrated Security=true;";// User ID=sa;Password=demol23";
                SqlConnection conn = new SqlConnection(connectionString);
                SqlDataAdapter data = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM STUDENTS ORDER BY LastName ASC";
                data.SelectCommand = cmd;
                conn.Open();
                data.Fill(ds);
                conn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Bad character:" + ex.ToString());
            }
            return ds;

        }
        public int InsertStudent(string firstname,string lastname,string email,string phone)
	        {
                int result = -1;
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        String query = "INSERT INTO dbo.Students ( LastName, FirstName, Email, Phone)OUTPUT Inserted.StudentID VALUES (@LastName, @FirstName, @Email, @Phone) ";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@LastName", lastname);
                            command.Parameters.AddWithValue("@FirstName", firstname);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Phone", !String.IsNullOrEmpty(phone) ? phone : (object)DBNull.Value);
                            connection.Open();
                            result = (int)command.ExecuteScalar(); 
                            connection.Close();
                           if (result < 0) Console.WriteLine("Error inserting data into Database!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Bad character:" + ex.ToString());
                }
                return result;
            }
	    public void DeleteStudent(int studentid)
	    {
		    try
		    {
			    using (SqlConnection connection = new SqlConnection(connectionString))
			    {
				    String query = "Delete From Students Where StudentID = @StudentID";
				    using (SqlCommand cmd = new SqlCommand(query, connection))
				    {
					    cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentid;
					    connection.Open();
					    cmd.ExecuteNonQuery();
                        connection.Close();
				    }
			    }
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine("Bad character:" + ex.ToString());
		    }
	    }	
	    public void UpdateStudent(int studentid, string firstname, string lastname, string email, string phone)
	    {
		    using (SqlConnection connection = new SqlConnection(connectionString))
		    {
			    String query = "UPDATE dbo.Students SET LastName=@LastName, FirstName=@FirstName, Email=@Email, Phone=@Phone  WHERE  StudentID = @StudentID";		
			    using (SqlCommand command = new SqlCommand(query, connection))
			    {
				    command.Parameters.AddWithValue("@StudentID", studentid);
				    command.Parameters.AddWithValue("@LastName", lastname);
				    command.Parameters.AddWithValue("@FirstName", firstname);
				    command.Parameters.AddWithValue("@Email", email);
				    command.Parameters.AddWithValue("@Phone",  !String.IsNullOrEmpty(phone) ?phone: (object)DBNull.Value);
                    connection.Open();
				    int result = command.ExecuteNonQuery();
                    connection.Close();//
				    // Check Error
				    if (result < 0)
					    Console.WriteLine("Error updating data into Database!");
			    }
		    }
	    }
	    
}




	
	


	
