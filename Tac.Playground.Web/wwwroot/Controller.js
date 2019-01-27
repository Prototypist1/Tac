const app = new Vue({
    el: '#app',
    data: {
        code: "entry-point { } ; module { } ;",
        run: function () {
            console.log(this.code);
        }
    }
});
