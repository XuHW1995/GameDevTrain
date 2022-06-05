/*
 * @lc app=leetcode.cn id=704 lang=csharp
 *
 * [704] 二分查找
 *
 * https://leetcode-cn.com/problems/binary-search/description/
 *
 * algorithms
 * Easy (54.40%)
 * Likes:    847
 * Dislikes: 0
 * Total Accepted:    614.7K
 * Total Submissions: 1.1M
 * Testcase Example:  '[-1,0,3,5,9,12]\n9'
 *
 * 给定一个 n 个元素有序的（升序）整型数组 nums 和一个目标值 target  ，写一个函数搜索 nums 中的
 * target，如果目标值存在返回下标，否则返回 -1。
 * 
 * 
 * 示例 1:
 * 
 * 输入: nums = [-1,0,3,5,9,12], target = 9
 * 输出: 4
 * 解释: 9 出现在 nums 中并且下标为 4
 * 
 * 
 * 示例 2:
 * 
 * 输入: nums = [-1,0,3,5,9,12], target = 2
 * 输出: -1
 * 解释: 2 不存在 nums 中因此返回 -1
 * 
 * 
 * 
 * 
 * 提示：
 * 
 * 
 * 你可以假设 nums 中的所有元素是不重复的。
 * n 将在 [1, 10000]之间。
 * nums 的每个元素都将在 [-9999, 9999]之间。
 * 
 * 
 */

// @lc code=start
public class Solution {
    public int Search(int[] nums, int target) {
        int left = 0;
        //左右都是闭区间
        int right = nums.Length - 1;
        
        while(left <= right)
        {
            //int middle = (right - left)/2 - left;
            //防止溢出？
            int middle = (left + right)/2;

            int middleNum = nums[middle];

            if(middleNum == target)
            {
                return middle;
            }
            //目标在右区间，且当前middle不是目标，则下次检测区间变成
            //[middle + 1, right]
            else if(middleNum < target)
            {
                left = middle + 1;
            }
            //目标在左区间，且当前middle不是目标，则下次检测区间变成
            //[left, middle - 1]
            else if(middleNum > target)
            {
                right = middle - 1;
            }
        }

        return -1;
    }
}
// @lc code=end

