# QuerySheper Web UI Guide

## 🎨 Beautiful Web Interface

Your QuerySheper now has a **beautiful, interactive web interface** that makes it super easy to execute SQL queries across multiple databases!

## 🚀 How to Access

1. **Run the application**:
   ```bash
   dotnet run
   ```

2. **Open your browser** to: `http://localhost:5163/`

3. **The beautiful UI will load automatically** with:
   - ✅ Modern gradient design
   - ✅ Interactive SQL query editor
   - ✅ Real-time results display
   - ✅ Quick example buttons
   - ✅ Auto-save functionality

## 🎯 Features

### **Interactive SQL Editor**
- ✅ **Large text area** for writing SQL queries
- ✅ **Auto-save** - Your queries are saved automatically
- ✅ **Syntax highlighting** ready (monospace font)
- ✅ **Resizable** text area for long queries

### **Quick Examples**
Click any example button to instantly populate the editor:
- **Basic Test**: `SELECT 1 as TestValue, 'Hello QuerySheper!' as Message`
- **Database Info**: `SELECT @@VERSION as DatabaseVersion, DB_NAME() as DatabaseName`
- **User Count**: `SELECT COUNT(*) as UserCount FROM Users`
- **Error Test**: `SELECT * FROM NonExistentTable`

### **Real-Time Results**
- ✅ **Loading animation** while executing
- ✅ **Summary statistics** (successful/failed databases)
- ✅ **Detailed results** for each database
- ✅ **Execution times** for performance monitoring
- ✅ **Error messages** with clear formatting
- ✅ **Data display** in JSON format

### **Modern Design**
- ✅ **Responsive design** - works on all devices
- ✅ **Glassmorphism effects** - modern UI trends
- ✅ **Smooth animations** - professional feel
- ✅ **Color-coded results** - green for success, red for errors
- ✅ **Professional typography** - easy to read

## 🎮 How to Use

### **Step 1: Enter SQL Query**
1. Type your SQL query in the text area
2. Or click one of the example buttons
3. Your query is automatically saved

### **Step 2: Execute**
1. Click **"🚀 Execute Query on All Databases"**
2. Watch the loading animation
3. See results appear in real-time

### **Step 3: View Results**
1. **Summary**: See total databases, successful, failed, execution time
2. **Database Results**: Each database shows:
   - ✅ **Success/Error status**
   - ⏱️ **Execution time**
   - 📊 **Row count** (for SELECT queries)
   - 📄 **Data or error message**

### **Step 4: Clear Results**
- Click **"🗑️ Clear Results"** to start fresh

## 📊 Example Results

When you execute `SELECT 1 as TestValue, 'Hello QuerySheper!' as Message`, you'll see:

```
📊 Query Results
┌─────────────────────────────────────┐
│  ✅ 1 Successful  ❌ 1 Failed       │
│  🗄️ 2 Total Databases              │
│  ⏱️ 00:00:00.169 Execution Time     │
└─────────────────────────────────────┘

🗄️ Database Results:
┌─ PostgreSQL_Default ─────────────────┐
│ ✅ SUCCESS                          │
│ ⏱️ Execution Time: 00:00:00.123     │
│ 📊 Rows: 1                          │
│ 📄 Data: [{"TestValue": 1, "Message": "Hello QuerySheper!"}] │
└─────────────────────────────────────┘

┌─ SqlServer_Default ──────────────────┐
│ ❌ ERROR                            │
│ 📄 Error: Connection failed         │
└─────────────────────────────────────┘
```

## 🔧 API Information

The UI also shows API information:
- **Endpoint**: `POST /api/simplequery/execute`
- **Content-Type**: `application/json`
- **Body**: SQL query as JSON string
- **Example curl command**

## 🎨 UI Features

### **Responsive Design**
- ✅ **Desktop**: Full-width layout with side-by-side results
- ✅ **Tablet**: Optimized for touch interaction
- ✅ **Mobile**: Stacked layout for small screens

### **Interactive Elements**
- ✅ **Hover effects** on buttons
- ✅ **Smooth transitions** between states
- ✅ **Loading spinners** during execution
- ✅ **Auto-scroll** to results
- ✅ **Form validation** (prevents empty queries)

### **Professional Styling**
- ✅ **Modern gradients** (purple to blue)
- ✅ **Glassmorphism cards** with backdrop blur
- ✅ **Golden accent colors** for highlights
- ✅ **Clean typography** with proper hierarchy
- ✅ **Consistent spacing** and alignment

## 🚀 Perfect For

- **Database Administrators**: Quick testing and validation
- **Developers**: Cross-database query execution
- **QA Teams**: Data comparison across environments
- **Business Users**: Simple query interface
- **Demos**: Professional-looking interface for presentations

## 🎉 Ready to Use!

Your QuerySheper now has a **professional, beautiful web interface** that makes multi-database querying as easy as clicking a button!

Just run `dotnet run` and open `http://localhost:5163/` to start using your new UI! 🚀
