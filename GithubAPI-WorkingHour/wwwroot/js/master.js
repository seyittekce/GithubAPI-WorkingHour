Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}
Date.prototype.DateToShortDateString = function () {
    var date = new Date(this.valueOf());
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    if (day < 10) {
        day = 0 + day;
    }
    if (month < 10) {
        month = 0 + month;
    }
    return day + "." + month + "." + year;
}
function getDates(startDate, endDate){
    var dateArray = new Array();
    startDate = new Date(startDate);
    endDate = new Date(endDate);
    var currentDate = startDate;
    while (currentDate <= endDate) {
        dateArray.push(new Date(currentDate));
        currentDate = currentDate.addDays(1);
    }
    return dateArray;
}