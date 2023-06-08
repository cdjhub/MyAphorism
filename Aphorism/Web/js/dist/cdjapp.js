
class CDJView{
    constructor(){
        this.$body = undefined;
        this.initHTML();
    }

    initHTML(){}

    getHTML(){
        return this.$body;
    }

    hide(){
        this.$body.hide();
    }

    show(){
        this.$body.show();
    }
}
class CDJSonView extends CDJView{
    constructor(root){
        super();
        
        this.root = root;
        this.registerHTML();
    }

    registerHTML(){
        this.root.$body.append(this.$body);
    }
}
class Display extends CDJSonView{
    constructor(root){
        super(root);
        
        this.lastAphorism = "";
        this.start();
    }

    start(){
        this.addListener();
        let str = "";
        for(let i = 0; i < 80; i ++ )
            str += "寄";
        this.showInfo(str);
        console.log("display start !");
    }

    addListener(){
        let outer = this;
        this.$body.click(function(){
            // Formium.hostWindow.sizeTo(100, 100);
            outer.root.hide_display();
        });
    }

    initHTML(){
        this.$body = $(`
        <div class="show-body context set-no-drag">
        </div>
        `);
    }

    showInfo(info){
        this.$body.text(info);
    }

    showAphorism(info) {
        this.lastAphorism = info;
        this.showInfo(info);
    }

    recoverAphorism(){
        this.showInfo(this.lastAphorism);
    }
}
class TaskPage extends CDJSonView{
    constructor(root){
        super(root);
        
        this.$showtaskpage = this.$body.find(".showtaskpage");
        this.$showtaskpage.hide();
        this.$select = this.$body.find(".taskpage-select");
        this.$timespan_page = this.$body.find(".taskpage-input-timespan");
        this.$timestamp_page = this.$body.find(".taskpage-input-timestamp");
        this.$timespan = this.$body.find(".input-timespan");
        this.$timespan_isloop = this.$body.find("#task-isLoop");
        this.$timestamp = this.$body.find(".input-timestamp");
        this.$submit = this.$body.find(".submit");
        this.$text = this.$body.find(".taskpage-input-text textarea");
        this.$timestamp_page.hide();

        this.lastTaskMsg = "";
        this.start();
    }

    start(){
        this.addListener();
        console.log("taskpage start !");
        console.log(this.$timespan_isloop);
    }

    getData(){
        let selectVal = this.$select.val();
        let res = {};
        res["Type"] = selectVal;
        res["Loop"] = 0;
        if(this.$timespan_isloop.is(':checked'))
            res["Loop"] = 1;
        // 获得时间秒数
        if(selectVal === "TimeSpan"){
            // 转化为秒
            let radioVal = this.$body.find('input[name=hms]:checked').val();
            radioVal = parseFloat(radioVal);
            let inputVal = this.$timespan.val();
            inputVal = parseFloat(inputVal);
            res["Time"] = radioVal * inputVal;
        } else if(selectVal === "TimeStamp"){
            let val = this.$timestamp.val();
            // 获得输入时间
            let hours = parseInt(val.substring(0, 2));
            let min = parseInt(val.substring(3, 5));
            // 获得当前时间
            let now = new Date();
            let nowh = parseInt(now.getHours());
            let nowm = parseInt(now.getMinutes());
            let nows = parseInt(now.getSeconds());
            // 计算差值
            if(hours == nowh){
                if(nowm >= min) 
                    res["Time"] = -1;
                else
                    res["Time"] = (min - nowm) * 60 - nows;
            } else {
                res["Time"] = this.calc_time(hours, min, nowh, nowm, nows);
            }
        }

        // 获得文本内容
        if(res["Time"] !== -1) {
            res["Text"] = this.$text.val();
        } else {
            res["Text"] = "error!";
        }
        console.log(res);
        return res;
    }

    // 计算两个时间的差值
    calc_time(nexth, nextm, nowh, nowm, nows){
        let diffh = nexth - nowh;
        if(diffh< 0)
            diffh += 24;
        let diffm = nextm - nowm;
        // 分钟不够小时借位
        if(diffm < 0){
            diffh --;
            diffm += 60;
        }
        return diffh * 3600 + diffm * 60 - nows;
    }

    addListener(){
        let outer = this;
    
        this.$select.change(function(){
            let val = outer.$select.val();
            if(val === "TimeSpan"){
                outer.$timestamp_page.hide();
                outer.$timespan_page.show();
            } else if (val === "TimeStamp"){
                outer.$timespan_page.hide();
                outer.$timestamp_page.show();
            }
        });

        this.$submit.click(function() {
            outer.send_task();
        });
    }

    // 发送Task
    send_task(){
        let outer = this;
        let tempStr = "";
        $.ajax({
            url: "http://api.app.local/task/addtask",
            type: "POST",
            data: JSON.stringify(outer.getData()),
            contentType: "application/json",
            async: false,
            success: function(result){
                if(result["result"] === "success"){
                    alert("添加成功");
                } else {
                    alert(result["result"]);
                }
            }
        });
    }

    initHTML(){
        this.$body = $(`
        <div class="show-body settaskpage set-no-drag">
            <div class="taskpage-left taskpage-son">
                <select class="taskpage-select">
                    <option value="TimeSpan">一段时间后提醒</option>
                    <option value="TimeStamp">某个时刻提醒</option>
                </select>
                <div class="taskpage-input-time">
                    <div class="taskpage-input-timespan">
                        <input class="input-timespan input-in-line" type="number">
                        <input class="input-in-line" type="radio" name="hms" value="3600">
                        <label class="input-in-line">h</label>
                        <input class="input-in-line" type="radio" name="hms" value="60" checked>
                        <label class="input-in-line">m</label>
                        <input class="input-in-line" type="radio" name="hms" value="1">
                        <label class="input-in-line">s</label>
                        <input class="input-in-line" type="checkbox" id="task-isLoop">
                        <label class="input-in-line">循环</label>
                    </div>
                    <div class="taskpage-input-timestamp">
                        <input class="input-timestamp" type="time">
                    </div>
                </div>
                <button class="submit">添加提醒</button>
            </div>
            <div class="taskpage-right taskpage-son">
                <div class="taskpage-input-text">
                    <textarea></textarea>
                </div>
            </div>
        </div>
        `);
    }
}
/*
配置好每个按钮的通用模板，其实也就是一些按钮事件和名称
每个按钮只需要继承，并且编写GetData函数即可
*/
class BaseToolBar{
    constructor($container, text){
        this.$container = $container;
        // 初始化进度条
        this.progressbar = undefined;
        if(text !== undefined)
            this.progressbar = new ProgressBar($container, text);
        else
            this.progressbar = new ProgressBar($container);

        // 初始化Body
        this.$body = undefined;
        this.initHTML();

        this.addListener();
    }
    
    addListener(){
        let outer = this;
        this.$body.click(function(){
            outer.clickFunc();
        });
    }

    clickFunc(){}

    // 间隔一段事件就轮询一次cpu信息或者内存信息
    intervalFunc(outer){
        outer.getData();
        setTimeout(outer.intervalFunc(outer), 1000);
    }

    // 设置进度条和提示信息
    setProgress(num){
        this.progressbar.setProgress(num);
        this.progressbar.setTitle(num.toFixed(2));
    }

    show(){
        this.progressbar.show();
    }

    hide(){
        this.progressbar.hide();
    }

    getHTML(){
        this.progressbar.getHTML();
    }

    initHTML(){
        this.$body = this.progressbar.getHTML();
    }
}
class CPUBar extends BaseToolBar{
    constructor(root, $container){
        super($container, "CPU");
        this.root = root;

        console.log("CPU Bar Star");
        // 平滑处理一下
        this.mooth = new Array();
        // 开始轮询
        setInterval(this.get_cpuoccupied.bind(this), 2000);
    }

    // 点击事件
    clickFunc(){
        if(this.root.displayIsShow)
            this.root.hide_display();
        else
            this.root.show_display();
    }

    // 获得CPU数据
    get_cpuoccupied(){
        let outer = this;
        $.ajax({
            url: "http://api.app.local/system/cpu",
            type: "GET",
            async: false,
            success: function(result){
                result = parseFloat(result);
                let ocp = 100 - result;
                //ocp = outer.getCpuUsed(ocp);

                // 显示在进度条上
                outer.setProgress(ocp);
            }
        });
    }

    // num占两份，权重更高些
    getCpuUsed(num){
        this.addCpuUsed(num);
        let sum = num;
        for(let i = 0; i < this.mooth.length; i ++){
            sum += this.mooth[i];
        }
        return sum /= (this.mooth.length + 1);
    }

    // 添加一个占用信息
    addCpuUsed(num){
        if(this.mooth.length == 5)
            this.mooth.shift();
        this.mooth.push(num);
    }
}
class MemBar extends BaseToolBar{
    constructor($container){
        super($container, "MEM");

        console.log("Mem Bar Star");
        // 开始轮询
        setInterval(this.get_memoccupied.bind(this), 2000);
    }

    // 获得MEM数据
    get_memoccupied(){
        let outer = this;
        $.ajax({
            url: "http://api.app.local/system/mem",
            type: "GET",
            async: false,
            success: function(result){
                result = parseFloat(result);
                // console.log(result);
                outer.setProgress(result);
            }
        });
    }
}
/*
进度条，需要一个container，然后直接放进去就行了
*/
class ProgressBar extends CDJView {
    constructor($container, text){
        super();

        this.$container = $container;
        this.registerHTML();

        this.$bar = this.$body.find("#myBar");
        this.$text = this.$body.find("#text");

        if(text !== undefined){
            this.setText(text);
        }
    }

    // 设置进度条百分比
    setProgress(num){
        this.$bar.width(`${num}%`);
    }

    // 设置显示内容
    setText(text){
        this.$text.text(`${text}`);
    }

    // 设置鼠标悬停提示信息
    setTitle(num){
        this.$container.attr('title', num + ' %');
    }

    initHTML(){
        this.$body = $(`
    <div class="progress-container">
        <div class="progress-bar" id="myBar"></div>
        <div class="progress-text" id="text"></div>
    </div>
        `);
    }

    registerHTML(){
        this.$container.append(this.$body);
    }
}
class TaskBar extends BaseToolBar{
    constructor(root, $container){
        super($container, "Task");
        this.root = root;

        this.progressbar.setTitle("展示事件持续时间为10s~");
        console.log("Task Bar Star");
    }

    // 点击事件
    clickFunc(){
        if(this.root.taskpageIsShow)
            this.root.hide_taskpage();
        else
            this.root.show_taskpage();
    }
}
class MainPage extends CDJView{
    constructor(){
        super();

        this.display = new Display(this);
        this.taskpage = new TaskPage(this);
        this.cpu = new CPUBar(this, $('.GetCpuOccupied'));
        this.mem = new MemBar($('.GetMemOccupied'));
        this.mem = new TaskBar(this, $('.Task'));
        this.group = $('.info-group');
        this.displayIsShow = true;
        this.taskpageIsShow = true;

        // 设置展开后窗体大小
        this.windowsWidth = 700;
        this.windowsHeight = 90;
        // this.sizeTo(this.windowsWidth, this.windowsHeight);

        // 展开后的X轴，Y轴坐标
        this.expandX = 1200;
        this.expandY = 930;

        // 收起后的X轴，Y轴坐标
        this.collapsX = 1750;
        this.collapsY = 930;
        // this.moveTo(this.expandX, this.expandY);
        this.hide_display();
        this.hide_taskpage();

        this.start();
    }

    start(){
        this.cpu.show();
        this.mem.show();
        console.log("MainPage start !");
        // this.show_display_info();
    }

    moveTo(x, y){
        Formium.hostWindow.moveTo(x, y);
    }

    sizeTo(width, height){
        Formium.hostWindow.sizeTo(width, height);
    }
    
    // 隐藏任务设置窗口
    hide_taskpage(){
        if(!this.taskpageIsShow)
            return ;
        this.taskpageIsShow = false;
        
        this.group.removeClass("width45px");
        this.taskpage.hide();
        this.group.addClass("width100");
        this.sizeTo(60, 90);
        this.moveTo(this.collapsX, this.collapsY);
    }

    // 展开任务设置窗口
    show_taskpage(){
        if(this.taskpageIsShow)
            return ;
        this.taskpageIsShow = true;
        this.hide_display();

        this.group.removeClass("width100");
        this.group.addClass("width45px");
        this.taskpage.show();
        this.sizeTo(this.windowsWidth, this.windowsHeight);
        this.moveTo(this.expandX, this.expandY);
    }

    // 收起文本窗口
    hide_display(){
        if(!this.displayIsShow)
            return ;
        this.displayIsShow = false;

        this.group.removeClass("width45px");
        this.display.hide();
        this.group.addClass("width100");
        this.sizeTo(60, 90);
        this.moveTo(this.collapsX, this.collapsY);
    }

    // 展开文本窗口
    show_display(){
        if(this.displayIsShow)
            return ;
        this.displayIsShow = true;
        // 隐藏Task窗口
        this.hide_taskpage();
        
        this.group.removeClass("width100");
        this.group.addClass("width45px");
        this.display.show();
        this.sizeTo(this.windowsWidth, this.windowsHeight);
        this.moveTo(this.expandX, this.expandY);
    }

    // 展示名言，自动打开，自动收起
    show_aphorism(info){
        if(info === undefined)
            return ;
        this.display.showAphorism(info);
        this.show_display();
        // 延迟10s收起来
        setTimeout(this.hide_display.bind(this), 10000);
    }   

    // 展示提醒
    show_taskInfo(info){
        if(info === undefined) 
            return ;
        this.taskpage.lastTaskMsg = info;
        this.display.showInfo(info);
        this.show_display();
        // 延迟10s收起来
        setTimeout(this.hide_display.bind(this), 5000);
        let outer = this;
        setTimeout(function(){outer.display.recoverAphorism();}, 5000);
    }

    initHTML(){
        this.$body = $('.cdjapp');
    }
}
